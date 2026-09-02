using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Drives the almanac screen: populates weapon/item entries, tracks the current selection,
/// updates the description/footer text, and shows weapon stat previews for discovered weapons.
/// </summary>
public class AlmanacController : MonoBehaviour {
    [SerializeField] public SimpleFadeIn simpleFadeIn; // fade controller for scene transitions and locked-state checks
    [SerializeField] private AlmanacStats almanacStats; // right-side stat preview that only appears for discovered weapons
    [SerializeField] private Sprite releasedButton; // default sprite for the page-swap buttons
    [SerializeField] private Sprite pressedButton; // depressed sprite for the page-swap buttons while held
    [SerializeField] private GameObject leftButton; // button object that moves on/off screen when the previous page exists
    [SerializeField] private GameObject rightButton; // button object that moves on/off screen when the next page exists
    [SerializeField] private TextMeshProUGUI bottomText; // footer text showing discovery totals and per-entry find counts
    [SerializeField] private float keyRepeatDelay = 0.35f; // first delay before held arrow keys begin repeating
    [SerializeField] private float keyRepeatInterval = 0.08f; // repeat cadence once a navigation key is held
    // world-space layout config; tune these in the inspector after placing the scene
    [SerializeField] public float xStart   = -4f; // leftmost spawn position for almanac entries
    [SerializeField] public float yStart   =  4f; // top row spawn position for almanac entries
    [SerializeField] public float xSpacing =  1f; // horizontal gap between neighboring entries
    [SerializeField] public float ySpacing =  1.3f; // vertical gap between rows
    public int page = 0; // active almanac page, where 0 = weapons and 1 = items

    private const int ItemsPerRow = 9; // grid width shared by both pages
    private const int MaxPage = 1; // there are currently only two almanac pages
    private const string UnknownName = "???"; // display name for undiscovered entries
    private const string UnknownDescription = "not yet discovered"; // description for undiscovered entries
    private Scripts s; // central service locator used throughout the project
    private int selRow = 0; // keyboard-selected row within the active almanac grid
    private int selCol = 0; // keyboard-selected column within the active almanac grid
    private int lastSyncedFlat = -1; // last flat index mirrored from highlight state to avoid redundant work
    private int lastPreviewIndex = -1; // last weapon index shown in the stat preview pane
    private int lastPreviewPage = -1; // page the current preview belongs to so page swaps invalidate it
    private List<string> curPageEntries = new(); // ordered names for the entries currently spawned on screen
    private bool preventPlayingFX = true; // suppresses startup clicks while the page first populates
    private KeyCode heldNavKey = KeyCode.None; // currently repeating navigation key, if any
    private float nextRepeatAt = 0f; // next unscaled time when held navigation should repeat

    /// <summary>
    /// Returns the number of grid rows needed to display the current page.
    /// </summary>
    private int TotalRows => Mathf.CeilToInt((float)curPageEntries.Count / ItemsPerRow);

    /// <summary>
    /// Initializes almanac state, clears any leftover floor items from the prior scene,
    /// then spawns the active page and its selection state.
    /// </summary>
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        s.itemManager.isAlmanac = true;
        simpleFadeIn = FindFirstObjectByType<SimpleFadeIn>();
        if (almanacStats == null) { almanacStats = FindFirstObjectByType<AlmanacStats>(); }
        if (bottomText != null) {
            bottomText.gameObject.SetActive(PlayerPrefs.GetString(s.BUTTONS_KEY) != "on");
        }
        ClearItems(); // clear legacy items from previous scene
        StartCoroutine(AllowFX());
        PopulatePage();
        UpdatePageButtons();
    }

    /// <summary>
    /// Delays menu sound playback so scene setup does not trigger stray click audio.
    /// </summary>
    private IEnumerator AllowFX() {
        yield return s.delays[0.45f];
        preventPlayingFX = false;
    }

    /// <summary>
    /// Keeps mouse and keyboard selection in sync, then processes held-key navigation.
    /// </summary>
    private void Update() {
        // keep internal row/col in sync if the user clicked something with the mouse
        SyncSelectionFromHighlight();

        HandleNavigationInput();
    }

    /// <summary>
    /// Handles one-shot and held-repeat arrow-key navigation for the almanac grid.
    /// </summary>
    private void HandleNavigationInput() {
        KeyCode currentKey = GetCurrentNavKey();
        if (currentKey == KeyCode.None) {
            heldNavKey = KeyCode.None;
            return;
        }

        if (Input.GetKeyDown(currentKey) || currentKey != heldNavKey) {
            // changing direction should feel like a fresh press instead of waiting for repeat delay
            heldNavKey = currentKey;
            nextRepeatAt = Time.unscaledTime + keyRepeatDelay;
            ApplyNavKey(currentKey);
            return;
        }

        if (Time.unscaledTime >= nextRepeatAt) {
            nextRepeatAt = Time.unscaledTime + keyRepeatInterval;
            ApplyNavKey(currentKey);
        }
    }

    /// <summary>
    /// Returns the first currently held navigation key, or `KeyCode.None` if none are held.
    /// </summary>
    /// <returns>the active arrow key being held</returns>
    private KeyCode GetCurrentNavKey() {
        if (Input.GetKey(KeyCode.LeftArrow)) { return KeyCode.LeftArrow; }
        if (Input.GetKey(KeyCode.RightArrow)) { return KeyCode.RightArrow; }
        if (Input.GetKey(KeyCode.UpArrow)) { return KeyCode.UpArrow; }
        if (Input.GetKey(KeyCode.DownArrow)) { return KeyCode.DownArrow; }
        return KeyCode.None;
    }

    /// <summary>
    /// Dispatches a navigation key press to the matching row/column movement helper.
    /// </summary>
    /// <param name="key">held or newly pressed arrow key</param>
    private void ApplyNavKey(KeyCode key) {
        if (key == KeyCode.LeftArrow) { MoveCol(-1); return; }
        if (key == KeyCode.RightArrow) { MoveCol(1); return; }
        if (key == KeyCode.UpArrow) { MoveRow(-1); return; }
        if (key == KeyCode.DownArrow) { MoveRow(1); }
    }

    /// <summary>
    /// Moves the selection horizontally across the flattened grid, wrapping at page edges.
    /// </summary>
    /// <param name="dir">-1 for left, 1 for right</param>
    private void MoveCol(int dir) {
        int flat = selRow * ItemsPerRow + selCol;
        flat = (flat + dir + curPageEntries.Count) % curPageEntries.Count;
        selRow = flat / ItemsPerRow;
        selCol = flat % ItemsPerRow;
        UpdateSelection();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    /// <summary>
    /// Moves the selection vertically without wrapping beyond the first or last row.
    /// </summary>
    /// <param name="dir">-1 for up, 1 for down</param>
    private void MoveRow(int dir) {
        int newRow = selRow + dir;
        if (newRow < 0 || newRow >= TotalRows) { return; }
        selRow = newRow;
        selCol = Mathf.Clamp(selCol, 0, GetRowItemCount(selRow) - 1);
        UpdateSelection();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    /// <summary>
    /// Returns how many entries exist in a given row of the current page.
    /// </summary>
    /// <param name="row">zero-based row index</param>
    /// <returns>number of entries in that row, accounting for a partial last row</returns>
    private int GetRowItemCount(int row) {
        int start = row * ItemsPerRow;
        return Mathf.Min(ItemsPerRow, curPageEntries.Count - start);
    }

    /// <summary>
    /// Moves the shared highlight to the current grid selection and refreshes all preview text.
    /// </summary>
    private void UpdateSelection() {
        int flat = selRow * ItemsPerRow + selCol;
        if (flat < 0 || flat >= s.itemManager.floorItems.Count) { return; }
        lastSyncedFlat = flat;
        GameObject obj = s.itemManager.floorItems[flat];
        s.itemManager.highlight.transform.position = obj.transform.position;
        s.itemManager.highlightedItem = obj;
        s.itemManager.itemDesc.text = GetDisplayText(flat);
        UpdateBottomText(flat);
        UpdateWeaponPreview(flat);
    }

    /// <summary>
    /// Mirrors mouse-driven highlight changes back into the keyboard row/column state.
    /// </summary>
    private void SyncSelectionFromHighlight() {
        if (s.itemManager.highlightedItem == null) { return; }
        int flat = s.itemManager.floorItems.IndexOf(s.itemManager.highlightedItem);
        if (flat < 0) { return; }
        selRow = flat / ItemsPerRow;
        selCol = flat % ItemsPerRow;
        if (flat == lastSyncedFlat) { return; }
        lastSyncedFlat = flat;
        UpdateBottomText(flat);
        UpdateWeaponPreview(flat);
    }

    /// <summary>
    /// Builds the description text for the selected entry, hiding undiscovered content.
    /// </summary>
    /// <param name="index">flat index within the active page</param>
    /// <returns>display text for the item description panel</returns>
    private string GetDisplayText(int index) {
        if (index >= curPageEntries.Count) { return ""; }
        bool known = page == 0
            ? Save.persistent.discoveredWeapons[index]
            : Save.persistent.discoveredItems[index];
        return known ? GetKnownDisplayText(curPageEntries[index]) : $"{UnknownName}\n{UnknownDescription}";
    }

    /// <summary>
    /// Resolves the full display text for a discovered almanac entry from `ItemManager`.
    /// </summary>
    /// <param name="entry">canonical item or weapon name in almanac order</param>
    /// <returns>name plus description lines for that entry</returns>
    private string GetKnownDisplayText(string entry) {
        return s.itemManager.GetDisplayTextForEntry(entry);
    }

    /// <summary>
    /// Switches between the weapon and item almanac pages.
    /// </summary>
    /// <param name="p">requested page index</param>
    public void SetPage(int p) {
        p = Mathf.Clamp(p, 0, MaxPage);
        if (p == page) { return; }
        page = p;
        ClearItems();
        PopulatePage();
        UpdatePageButtons();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    /// <summary>
    /// Steps one page left or right from the current almanac page.
    /// </summary>
    /// <param name="dir">-1 for previous page, 1 for next page</param>
    public void StepPage(int dir) {
        SetPage(page + dir);
    }

    /// <summary>
    /// Swaps one page button to its pressed sprite.
    /// </summary>
    /// <param name="leftOrRight">which side button to update</param>
    public void ChangeToPressed(string leftOrRight) {
        GameObject target = leftOrRight == "Left" ? leftButton : rightButton;
        if (target == null || pressedButton == null) { return; }
        target.GetComponent<AlmanacSwapButton>().spriteRenderer.sprite = pressedButton;
    }

    /// <summary>
    /// Swaps one page button back to its released sprite.
    /// </summary>
    /// <param name="leftOrRight">which side button to update</param>
    public void ChangeToReleased(string leftOrRight) {
        GameObject target = leftOrRight == "Left" ? leftButton : rightButton;
        if (target == null || releasedButton == null) { return; }
        target.GetComponent<AlmanacSwapButton>().spriteRenderer.sprite = releasedButton;
    }

    /// <summary>
    /// Moves page buttons off-screen when there is no page available in that direction.
    /// </summary>
    private void UpdatePageButtons() {
        if (leftButton != null) {
            leftButton.transform.position = page == 0
                ? new Vector2(-8.53f, 20f)
                : new Vector2(-8.53f, 1f);
        }
        if (rightButton != null) {
            rightButton.transform.position = page == MaxPage
                ? new Vector2(8.53f, 20f)
                : new Vector2(8.53f, 1f);
        }
    }

    /// <summary>
    /// Updates the footer text with discovered totals and the selected entry's find count.
    /// </summary>
    /// <param name="index">flat index of the selected entry</param>
    private void UpdateBottomText(int index) {
        if (bottomText == null || index < 0 || index >= curPageEntries.Count || Save.persistent == null) { return; }

        bool isWeaponPage = page == 0;
        bool[] discoveredArr = isWeaponPage ? Save.persistent.discoveredWeapons : Save.persistent.discoveredItems;
        int[] countArr = isWeaponPage ? Save.persistent.discoveredWeaponCounts : Save.persistent.discoveredItemCounts;
        string heading = isWeaponPage ? "weapons discovered" : "items discovered";
        int discoveredCount = 0;

        foreach (bool discovered in discoveredArr) {
            if (discovered) { discoveredCount++; }
        }

        if (!discoveredArr[index]) {
            bottomText.text = $"{heading}: {discoveredCount}/{curPageEntries.Count}\nyou've not found this yet";
            return;
        }

        bottomText.text = $"{heading}: {discoveredCount}/{curPageEntries.Count}\nyou've found {curPageEntries[index]} {countArr[index]} times";
    }

    /// <summary>
    /// Shows or hides the weapon stat preview pane for the current selection.
    /// </summary>
    /// <param name="index">flat index of the currently selected entry</param>
    private void UpdateWeaponPreview(int index) {
        if (index == lastPreviewIndex && page == lastPreviewPage) { return; }

        bool canPreview = page == 0
            && index >= 0
            && index < curPageEntries.Count
            && Save.persistent != null
            && Save.persistent.discoveredWeapons[index]
            && almanacStats != null;

        if (!canPreview) {
            HideWeaponPreview();
            return;
        }

        Item item = s.itemManager.floorItems[index].GetComponent<Item>();
        if (item == null || item.weaponStats == null || item.weaponStats.Count == 0) {
            HideWeaponPreview();
            return;
        }

        lastPreviewIndex = index;
        lastPreviewPage = page;
        almanacStats.ShowWeaponStats(item.weaponStats);
    }

    /// <summary>
    /// Clears the weapon preview pane and its cached selection bookkeeping.
    /// </summary>
    private void HideWeaponPreview() {
        lastPreviewIndex = -1;
        lastPreviewPage = -1;
        if (almanacStats != null) { almanacStats.Clear(); }
    }

    /// <summary>
    /// Destroys all spawned almanac item objects and resets local selection state.
    /// </summary>
    private void ClearItems() {
        foreach (GameObject g in s.itemManager.floorItems) {
            if (g != null) { Destroy(g); }
        }
        s.itemManager.floorItems.Clear();
        curPageEntries.Clear();
        selRow = 0;
        selCol = 0;
        lastSyncedFlat = -1;
        HideWeaponPreview();
    }

    /// <summary>
    /// Spawns the entries for the active page using the persistent discovery arrays.
    /// </summary>
    private void PopulatePage() {
        string[] entries = page == 0
            ? ItemManager.AlmanacWeaponOrder
            : ItemManager.AlmanacItemOrder;

        curPageEntries = new List<string>(entries);

        for (int i = 0; i < entries.Length; i++) {
            int r = i / ItemsPerRow;
            int c = i % ItemsPerRow;
            Vector2 pos = new(xStart + c * xSpacing, yStart - r * ySpacing);

            // almanac order and persistent discovery arrays intentionally share the same index mapping
            bool known = page == 0
                ? Save.persistent.discoveredWeapons[i]
                : Save.persistent.discoveredItems[i];

            GameObject obj = s.itemManager.CreateAlmanacItem(entries[i], known, page == 0, pos);
            s.itemManager.floorItems.Add(obj);
        }

        if (curPageEntries.Count > 0) { UpdateSelection(); }
    }
}

