using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlmanacController : MonoBehaviour {
    [SerializeField] public SimpleFadeIn simpleFadeIn;
    [SerializeField] private AlmanacStats almanacStats;
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI bottomText;
    [SerializeField] private float keyRepeatDelay = 0.35f;
    [SerializeField] private float keyRepeatInterval = 0.08f;
    // world-space layout config; tune these in the inspector after placing the scene
    [SerializeField] public float xStart   = -4f;
    [SerializeField] public float yStart   =  4f;
    [SerializeField] public float xSpacing =  1f;
    [SerializeField] public float ySpacing =  1.3f;
    public int page = 0; // 0 = weapons, 1 = items

    private const int ItemsPerRow = 9;
    private const int MaxPage = 1;
    private const string UnknownName = "???";
    private const string UnknownDescription = "not yet discovered";
    private Scripts s;
    private int selRow = 0;
    private int selCol = 0;
    private int lastSyncedFlat = -1;
    private int lastPreviewIndex = -1;
    private int lastPreviewPage = -1;
    private List<string> curPageEntries = new();
    private bool preventPlayingFX = true;
    private KeyCode heldNavKey = KeyCode.None;
    private float nextRepeatAt = 0f;

    private int TotalRows => Mathf.CeilToInt((float)curPageEntries.Count / ItemsPerRow);

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

    private IEnumerator AllowFX() {
        yield return s.delays[0.45f];
        preventPlayingFX = false;
    }

    private void Update() {
        // keep internal row/col in sync if the user clicked something with the mouse
        SyncSelectionFromHighlight();

        HandleNavigationInput();
    }

    private void HandleNavigationInput() {
        KeyCode currentKey = GetCurrentNavKey();
        if (currentKey == KeyCode.None) {
            heldNavKey = KeyCode.None;
            return;
        }

        if (Input.GetKeyDown(currentKey) || currentKey != heldNavKey) {
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

    private KeyCode GetCurrentNavKey() {
        if (Input.GetKey(KeyCode.LeftArrow)) { return KeyCode.LeftArrow; }
        if (Input.GetKey(KeyCode.RightArrow)) { return KeyCode.RightArrow; }
        if (Input.GetKey(KeyCode.UpArrow)) { return KeyCode.UpArrow; }
        if (Input.GetKey(KeyCode.DownArrow)) { return KeyCode.DownArrow; }
        return KeyCode.None;
    }

    private void ApplyNavKey(KeyCode key) {
        if (key == KeyCode.LeftArrow) { MoveCol(-1); return; }
        if (key == KeyCode.RightArrow) { MoveCol(1); return; }
        if (key == KeyCode.UpArrow) { MoveRow(-1); return; }
        if (key == KeyCode.DownArrow) { MoveRow(1); }
    }

    // called by left/right arrow keys; wraps across rows
    private void MoveCol(int dir) {
        int flat = selRow * ItemsPerRow + selCol;
        flat = (flat + dir + curPageEntries.Count) % curPageEntries.Count;
        selRow = flat / ItemsPerRow;
        selCol = flat % ItemsPerRow;
        UpdateSelection();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    // called by up/down arrow keys; clamps at page edges
    private void MoveRow(int dir) {
        int newRow = selRow + dir;
        if (newRow < 0 || newRow >= TotalRows) { return; }
        selRow = newRow;
        selCol = Mathf.Clamp(selCol, 0, GetRowItemCount(selRow) - 1);
        UpdateSelection();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    // number of items in the given row (last row may be partial)
    private int GetRowItemCount(int row) {
        int start = row * ItemsPerRow;
        return Mathf.Min(ItemsPerRow, curPageEntries.Count - start);
    }

    // move highlight and update description text to match (row, col)
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

    // re-sync row/col from the highlighted item (handles mouse clicks)
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

    // returns the text to show in itemDesc for the item at the given flat index
    private string GetDisplayText(int index) {
        if (index >= curPageEntries.Count) { return ""; }
        bool known = page == 0
            ? Save.persistent.discoveredWeapons[index]
            : Save.persistent.discoveredItems[index];
        return known ? GetKnownDisplayText(curPageEntries[index]) : $"{UnknownName}\n{UnknownDescription}";
    }

    // returns name + description line(s) for a known almanac entry
    private string GetKnownDisplayText(string entry) {
        return s.itemManager.GetDisplayTextForEntry(entry);
    }

    // called by page-switch UI buttons
    public void SetPage(int p) {
        p = Mathf.Clamp(p, 0, MaxPage);
        if (p == page) { return; }
        page = p;
        ClearItems();
        PopulatePage();
        UpdatePageButtons();
        if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
    }

    // called by side button controls to move between almanac pages
    public void StepPage(int dir) {
        SetPage(page + dir);
    }

    public void ChangeToPressed(string leftOrRight) {
        GameObject target = leftOrRight == "Left" ? leftButton : rightButton;
        if (target == null || pressedButton == null) { return; }
        target.GetComponent<AlmanacSwapButton>().spriteRenderer.sprite = pressedButton;
    }

    public void ChangeToReleased(string leftOrRight) {
        GameObject target = leftOrRight == "Left" ? leftButton : rightButton;
        if (target == null || releasedButton == null) { return; }
        target.GetComponent<AlmanacSwapButton>().spriteRenderer.sprite = releasedButton;
    }

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

    private void HideWeaponPreview() {
        lastPreviewIndex = -1;
        lastPreviewPage = -1;
        if (almanacStats != null) { almanacStats.Clear(); }
    }

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

    private void PopulatePage() {
        string[] entries = page == 0
            ? ItemManager.AlmanacWeaponOrder
            : ItemManager.AlmanacItemOrder;

        curPageEntries = new List<string>(entries);

        for (int i = 0; i < entries.Length; i++) {
            int r = i / ItemsPerRow;
            int c = i % ItemsPerRow;
            Vector2 pos = new(xStart + c * xSpacing, yStart - r * ySpacing);

            bool known = page == 0
                ? Save.persistent.discoveredWeapons[i]
                : Save.persistent.discoveredItems[i];

            GameObject obj = s.itemManager.CreateAlmanacItem(entries[i], known, page == 0, pos);
            s.itemManager.floorItems.Add(obj);
        }

        if (curPageEntries.Count > 0) { UpdateSelection(); }
    }
}

