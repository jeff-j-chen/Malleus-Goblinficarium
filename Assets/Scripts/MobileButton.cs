using UnityEngine;
using TMPro;

public class MobileButton : MonoBehaviour {
    private Scripts s;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshProUGUI text;
    private bool usingSR;
    private bool showButtons;
    
    private void Start() {
        s = FindObjectOfType<Scripts>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) { text = GetComponent<TextMeshProUGUI>(); }
        // if not a sprite (text button), use textmeshpro instead
        usingSR = spriteRenderer != null;
        // toggle bool for handling sprites or text objects
        gameObject.SetActive(PlayerPrefs.GetString(s.BUTTONS_KEY) == "on");
    }

    private void OnMouseUpAsButton() {
        switch (gameObject.name) {
            case "UButton": s.player.MoveTargetUp(); break;
            case "DButton": s.player.MoveTargetDown(); break;
            case "LButton": s.itemManager.SelectLeft(); break;
            case "use": s.itemManager.UseCurrentItem(); break;
            case "drop": s.itemManager.DropCurrentItem(); break;
            case "RButton": s.itemManager.SelectRight(); break;
            case "menu": s.backToMenu.GoBack(); break;
            case "items": s.itemManager.ChangeItemList(); break;
            case "select": s.characterSelector.Select(); break;
            case "easy mode": s.characterSelector.CycleDifficulty(); break;
            case "reset stats": s.statistics.ResetStats(); break;
        }
    }
    
    // various functions for making either the sprite or the text have some tactile feedback

    private void OnMouseEnter() {
        if (usingSR) {  spriteRenderer.color = Colors.hovered; }
        else { text.color = Colors.hovered; }
    }

    private void OnMouseDown() {
        if (gameObject.name != "LButton" && gameObject.name != "RButton") {
            // l and r buttons already play click when selecting an item
            s.soundManager.PlayClip("click0");
        }
        if (usingSR) {  spriteRenderer.color = Colors.clicked; }
        else { text.color = Colors.clicked; }
    }

    private void OnMouseExit() {
        if (usingSR) {  spriteRenderer.color = Color.white; }
        else { text.color = Color.white; }
    }

    private void OnMouseUp() {
        if (usingSR) {  spriteRenderer.color = Color.white; }
        else { text.color = Color.white; }
    }
    
    
}
