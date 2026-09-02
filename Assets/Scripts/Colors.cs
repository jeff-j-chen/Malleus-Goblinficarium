using UnityEngine;

/// <summary>
/// Central color lookup used by dice, stats, UI highlights, and almanac previews.
/// Array order matters and is shared across multiple systems.
/// </summary>
public static class Colors {
    public static readonly string[] colorNameArr = { "green", "blue", "red", "white", "yellow" }; // canonical stat/color names in shared index order
    public static Color green = new(0.357f, 0.651f, 0.498f); // gameplay green tint  #5BA67F
    public static Color blue = new(0.329f, 0.529f, 1.0f); // gameplay blue tint  #5487FF
    public static Color red = new(0.843f, 0.075f, 0.075f); // gameplay red tint  #D71313
    public static Color white = new(0.816f, 0.816f, 0.816f); // gameplay white tint  #D0D0D0
    public static Color yellow = new(1.0f, 0.82f, 0.443f); // gameplay yellow tint  #FFD171
    public static Color hovered = new(0.69f, 0.671f, 0.659f); // hover tint for menu/item highlighting  #B0ABA8
    public static Color clicked = new(0.522f, 0.502f, 0.49f); // pressed tint for clicks and button states  #85807D
    public static Color disabled = new(0.251f, 0.251f, 0.251f); // disabled tint for unavailable UI/actions  #404040
    public static readonly Color[] colorArr = { green, blue, red, white, yellow }; // color values aligned with `colorNameArr`
}