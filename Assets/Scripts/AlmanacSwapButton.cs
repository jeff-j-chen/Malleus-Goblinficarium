using UnityEngine;
public class AlmanacSwapButton : MonoBehaviour {
    [SerializeField] private string leftOrRight;
    [SerializeField] private Sprite releasedButton;
    [SerializeField] private Sprite pressedButton;
    private AlmanacController almanacController;
    public SpriteRenderer spriteRenderer;

    private void Start() {
        almanacController = FindFirstObjectByType<AlmanacController>();
    }

    private void OnMouseDown() {
        spriteRenderer.sprite = pressedButton;
        almanacController.ChangeToPressed(leftOrRight);
    }

    private void OnMouseUp() {
        spriteRenderer.sprite = releasedButton;
        if (leftOrRight == "Left") { almanacController.StepPage(-1); }
        else { almanacController.StepPage(1); }
        almanacController.ChangeToReleased(leftOrRight);
    }
}
