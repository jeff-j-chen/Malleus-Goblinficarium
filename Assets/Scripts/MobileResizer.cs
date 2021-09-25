using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class MobileResizer : MonoBehaviour {
    private Scripts scripts;
    private bool mobileMode;
    private readonly int desktopFontSize = 32;
    private readonly int mobileFontSize = 48;
    private readonly Vector3 menuIconDesktopScale = new(0.35f, 0.35f, 1f);
    private readonly Vector2[] menuIconDesktopPos = { 
        new(0f, 0.76f), 
        new(0f, -0.24f), 
        new(0f, -1.24f), 
        new(0f, -2.24f), 
        new(0f, -3.24f),
    };
    private readonly Vector2[] menuIconTextDesktopPos = { 
        new(-253.7f, -157.1f), 
        new(-253.7f, -186.6f), 
        new(-253.7f, -216.6f), 
        new(-253.7f, -246.8f), 
        new(-253.7f, -277.0f),
    };
    private readonly Vector3 menuIconMobileScale = new(0.5f, 0.5f, 1f);
    private readonly Vector2[] menuIconMobilePos = { 
        new(0.32f, -3.06f), 
        new(6.8f, -3.06f), 
        new(12.72f, -3.06f), 
        new(20.11f, -3.06f), 
        new(9.8f, -1.41f) 
    };
    private readonly Vector2[] menuIconTextMobilePos = { 
        new(-235.2f, -264.4f), 
        new(-43.1f, -264.4f), 
        new(141.2f, -264.4f), 
        new(357.7f, -264.4f), 
        new(47.4f, -213.23f),
    };
    
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        if (scripts.menuIcon != null) {
            FlipMenuIconMode();
        }
    }

    public void FlipMenuIconMode() {
        mobileMode = PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on";
        if (mobileMode) {
            for (int i = 0; i < 5; i++) {
                scripts.menuIcon.menuIconOrdering[i].transform.localScale = menuIconMobileScale;
                scripts.menuIcon.menuIconOrdering[i].transform.localPosition = menuIconMobilePos[i];
                scripts.menuIcon.menuIconTextOrdering[i].GetComponent<TextMeshProUGUI>().fontSize = mobileFontSize;
                scripts.menuIcon.menuIconTextOrdering[i].transform.localPosition = menuIconTextMobilePos[i];
            }
        }
        else {
            for (int i = 0; i < 5; i++) {
                scripts.menuIcon.menuIconOrdering[i].transform.localScale = menuIconDesktopScale;
                scripts.menuIcon.menuIconOrdering[i].transform.localPosition = menuIconDesktopPos[i];
                scripts.menuIcon.menuIconTextOrdering[i].GetComponent<TextMeshProUGUI>().fontSize = desktopFontSize;
                scripts.menuIcon.menuIconTextOrdering[i].transform.localPosition = menuIconTextDesktopPos[i];
            }
        }
    }
}
