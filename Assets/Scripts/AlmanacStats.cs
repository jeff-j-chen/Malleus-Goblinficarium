using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlmanacStats : MonoBehaviour {
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject negSquare;
    [SerializeField] private float xCoord = 11.5f;
    [SerializeField] private float xOffset = -0.6f;
    [SerializeField] private float[] yCoords = { 8.77f, 7.77f, 6.77f, 5.77f };

    private readonly List<GameObject> existingStatSquares = new();

    public void ShowWeaponStats(Dictionary<string, int> weaponStats) {
        Clear();
        if (weaponStats == null) { return; }

        for (int i = 0; i < 4; i++) {
            string statName = Colors.colorNameArr[i];
            int statValue = weaponStats.TryGetValue(statName, out int value) ? value : 0;
            SpawnStatRow(i, Colors.colorArr[i], statValue);
        }
    }

    public void Clear() {
        foreach (GameObject stat in existingStatSquares) {
            if (stat != null) { Destroy(stat); }
        }
        existingStatSquares.Clear();
    }

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
                if (!isPositive) { spriteRenderer.flipX = xOffset < 0f; }
            }
            existingStatSquares.Add(spawnedShape);
        }
    }
}
