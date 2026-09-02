using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Renders the compact weapon stat preview shown on the almanac weapon page.
/// </summary>
public class AlmanacStats : MonoBehaviour {
    [SerializeField] private GameObject square; // prefab used for positive stat pips
    [SerializeField] private GameObject negSquare; // prefab used for negative stat pips
    [SerializeField] private float xCoord = 11.5f; // x position of the first pip in each row
    [SerializeField] private float xOffset = -0.6f; // horizontal distance between neighboring pips
    [SerializeField] private float[] yCoords = { 8.77f, 7.77f, 6.77f, 5.77f }; // one y position per weapon stat row

    private readonly List<GameObject> existingStatSquares = new(); // spawned preview pips that must be cleaned up before redrawing

    /// <summary>
    /// Rebuilds the stat preview using the provided weapon stat dictionary.
    /// Missing colors are treated as zero-value rows.
    /// </summary>
    /// <param name="weaponStats">weapon stat values keyed by color name</param>
    public void ShowWeaponStats(Dictionary<string, int> weaponStats) {
        Clear();
        if (weaponStats == null) { return; }

        for (int i = 0; i < 4; i++) {
            string statName = Colors.colorNameArr[i];
            int statValue = weaponStats.TryGetValue(statName, out int value) ? value : 0;
            SpawnStatRow(i, Colors.colorArr[i], statValue);
        }
    }

    /// <summary>
    /// Destroys all currently spawned stat preview objects.
    /// </summary>
    public void Clear() {
        foreach (GameObject stat in existingStatSquares) {
            if (stat != null) { Destroy(stat); }
        }
        existingStatSquares.Clear();
    }

    /// <summary>
    /// Spawns one horizontal row of positive or negative stat pips.
    /// </summary>
    /// <param name="statIndex">which stat row to draw</param>
    /// <param name="statColor">color tint for the spawned pips</param>
    /// <param name="statValue">signed stat magnitude for that row</param>
    private void SpawnStatRow(int statIndex, Color statColor, int statValue) {
        int shapeCount = Mathf.Abs(statValue);
        bool isPositive = statValue >= 0;

        for (int i = 0; i < shapeCount; i++) {
            Vector3 position = new(xCoord + i * xOffset, yCoords[statIndex], 0f);
            GameObject prefab = isPositive ? square : negSquare;
            GameObject spawnedShape = Instantiate(prefab, position, Quaternion.identity, transform);
            SpriteRenderer spriteRenderer = spawnedShape.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.color = statColor;
                // negative pips are mirrored so they visually point toward the center of the panel
                if (!isPositive) { spriteRenderer.flipX = xOffset < 0f; }
            }
            existingStatSquares.Add(spawnedShape);
        }
    }
}
