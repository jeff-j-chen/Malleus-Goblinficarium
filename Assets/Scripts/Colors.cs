using UnityEngine;
public static class Colors {
    [SerializeField] public static string[] colorNameArr = { "green", "blue", "red", "white", "yellow" };
    [SerializeField] public static Color green = new Color(r:0.357f, g:0.651f, b:0.498f); // #5BA67F
    [SerializeField] public static Color blue = new Color(r:0.329f, g:0.529f, b:1.0f); // #5487FF
    [SerializeField] public static Color red = new Color(r:0.843f, g:0.075f, b:0.075f); // #D71313
    [SerializeField] public static Color white = new Color(r:0.816f, g:0.816f, b:0.816f); // #D0D0D0
    [SerializeField] public static Color yellow = new Color(r:1.0f, g:0.82f, b:0.443f); // #FFD171
    [SerializeField] public static Color hovered = new Color(r:0.69f, g:0.671f, b:0.659f); // #b0aba8
    [SerializeField] public static Color clicked = new Color(r:0.522f, g:0.502f, b:0.49f); // #85807d
    [SerializeField] public static Color disabled = new Color(r:0.251f, g:0.251f, b:0.251f); // #404040
    [SerializeField] public static Color[] colorArr = { green, blue, red, white, yellow };
}