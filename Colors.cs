using UnityEngine;

public class Colors : MonoBehaviour {
    public Color gray;
    [SerializeField] public string[] htmlCodes = new string[] { "#5BA67F", "#5487FF", "#D71313", "#D0D0D0", "#FFD171", };
    [SerializeField] public Color[] colorArr;
    [SerializeField] public string[] colorNameArr = new string[] { "green", "blue", "red", "white", "yellow" };
    public Color green, blue, red, white, yellow, hovered, clicked, disabled;

    private void Start() {
        ColorUtility.TryParseHtmlString("#5BA67F", out green);
        ColorUtility.TryParseHtmlString("#5487FF", out blue);
        ColorUtility.TryParseHtmlString("#D71313", out red);
        ColorUtility.TryParseHtmlString("#D0D0D0", out white);
        ColorUtility.TryParseHtmlString("#FFD171", out yellow);
        // KEEP IT AS STRINGS, FOR SOME REASON REFERENCING THE ARRAY BREAKS EVERYTHING
        colorArr = new Color[] { green, blue, red, white, yellow };
        ColorUtility.TryParseHtmlString("#b0aba8", out hovered);
        ColorUtility.TryParseHtmlString("#85807d", out clicked);
        ColorUtility.TryParseHtmlString("#404040", out disabled);
        // parse the html strings such that the colors are created as Color objects and given a variable
    }
}