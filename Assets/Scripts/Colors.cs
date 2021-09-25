using UnityEngine;
public static class Colors {
    public static readonly string[] colorNameArr = { "green", "blue", "red", "white", "yellow" };
    public static Color green = new(0.357f, 0.651f, 0.498f); // #5BA67F
    public static Color blue = new(0.329f, 0.529f, 1.0f); // #5487FF
    public static Color red = new(0.843f, 0.075f, 0.075f); // #D71313
    public static Color white = new(0.816f, 0.816f, 0.816f); // #D0D0D0
    public static Color yellow = new(1.0f, 0.82f, 0.443f); // #FFD171
    public static Color hovered = new(0.69f, 0.671f, 0.659f); // #b0aba8
    public static Color clicked = new(0.522f, 0.502f, 0.49f); // #85807d
    public static Color disabled = new(0.251f, 0.251f, 0.251f); // #404040
    public static readonly Color[] colorArr = { green, blue, red, white, yellow };
}