using UnityEngine;
using TMPro;

public class MobileButton : MonoBehaviour {
    private Scripts scripts;
    private SpriteRenderer spriteRenderer;
    private TextMeshProUGUI text;
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
        print($"button called from {gameObject.name}!");
        if (gameObject.name == "UButton") { scripts.player.MoveTargetUp();}
        else if (gameObject.name == "DButton") { scripts.player.MoveTargetDown(); }
        else if (gameObject.name == "LButton") { scripts.itemManager.SelectLeft(); }
        else if (gameObject.name == "use") { scripts.itemManager.UseCurrentItem(); }
        else if (gameObject.name == "drop") { scripts.itemManager.DropCurrentItem(); }
        else if (gameObject.name == "RButton") { scripts.itemManager.SelectRight(); }
        else if (gameObject.name == "menu") { }
        else if (gameObject.name == "restart") { }
    }
    
    // various functions for making either the sprite or the text have some tactile feedback

    private void OnMouseEnter() {
        if (usingSR) {  spriteRenderer.color = scripts.colors.hovered; }
        else { text.color = scripts.colors.hovered; }
    }

    private void OnMouseDown() {
        scripts.soundManager.PlayClip("click0");
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
