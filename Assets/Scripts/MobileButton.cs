using UnityEngine;
using TMPro;

public class MobileButton : MonoBehaviour {
    private Scripts scripts;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshProUGUI text;
    private bool usingSR;
    private bool showButtons;
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) { text = GetComponent<TextMeshProUGUI>(); }
        // if not a sprite (text button), use textmeshpro instead
        usingSR = spriteRenderer != null;
        // toggle bool for handling sprites or text objects
        gameObject.SetActive(PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on");
    }

    private void OnMouseUpAsButton() {
        switch (gameObject.name) {
            case "UButton": scripts.player.MoveTargetUp(); break;
            case "DButton": scripts.player.MoveTargetDown(); break;
            case "LButton": scripts.itemManager.SelectLeft(); break;
            case "use": scripts.itemManager.UseCurrentItem(); break;
            case "drop": scripts.itemManager.DropCurrentItem(); break;
            case "RButton": scripts.itemManager.SelectRight(); break;
            case "menu": scripts.backToMenu.GoBack(); break;
            case "restart": scripts.player.AttemptSuicide(); break;
            case "select": scripts.characterSelector.Select(); break;
            case "easy mode": scripts.characterSelector.ToggleEasy(); break;
            case "reset stats": scripts.statistics.ResetStats(); break;
        }
    }
    
    // various functions for making either the sprite or the text have some tactile feedback

    private void OnMouseEnter() {
        if (usingSR) {  spriteRenderer.color = scripts.colors.hovered; }
        else { text.color = scripts.colors.hovered; }
    }

    private void OnMouseDown() {
        if (gameObject.name != "LButton" && gameObject.name != "RButton") {
            // l and r buttons already play click when selecting an item
            scripts.soundManager.PlayClip("click0");
        }
        if (usingSR) {  spriteRenderer.color = scripts.colors.clicked; }
        else { text.color = scripts.colors.clicked; }
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
