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
        new(12.94f, -3.06f), 
        new(20.11f, -3.06f), 
        new(9.11f, -1.366f) 
    };
    private readonly Vector2[] menuIconTextMobilePos = { 
        new(-235.2f, -264.4f), 
        new(-43.1f, -264.4f), 
        new(141.2f, -264.4f), 
        new(357.7f, -264.4f), 
        new(20.4f, -213.23f),
    };
    [SerializeField] private GameObject[] menuButtons;
    private readonly Vector3 menuButtonDesktopScale = new(1f, 1f, 1f);
    private readonly Vector2[] menuButtonDesktopPos = { 
        new(46.6f, 45f), 
        new(46.6f, 15f), 
        new(46.6f, -15f), 
        new(46.6f, -45f), 
        new(46.6f, -75f), 
        new(46.6f, -105f), 
    };
    private readonly Vector3 menuButtonMobileScale = new(1.5f, 1.5f, 1f);
    private readonly Vector2[] menuButtonMobilePos = { 
        new(70.5f, 75f+15f), 
        new(70.5f, 30f+15f), 
        new(70.5f, -15f+15f), 
        new(70.5f, -60f+15f), 
        new(70.5f, -105f+15f), 
        new(70.5f, -150f+15f), 
    };
    [SerializeField] private GameObject titleText;
    private readonly Vector2 titleTextDesktopPos = new(0f, 200f);
    private readonly Vector2 titleTextMobilePos = new(0f, 205f);
    private readonly Vector3 titleTextDesktopScale = new(1f, 1f, 1f);
    [SerializeField] private GameObject subtitleText;
    private readonly Vector3 titleTextMobileScale = new(1.5f, 1.5f, 1f);
    private readonly Vector2 subtitleTextDesktopPos = new(0f, 175f);
    private readonly Vector2 subtitleTextMobilePos = new(0f, 175f);
    
    
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        if (scripts.menuIcon != null) {
            FlipMenuIconMode();
        }
    }

    public void FlipMenuIconMode() {
        mobileMode = PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on";
        if (!mobileMode) {
            // desktop mode
            SetMenuIcons(menuIconDesktopScale, menuIconDesktopPos, desktopFontSize, menuIconTextDesktopPos);
            SetMenuButtons(menuButtonDesktopScale, menuButtonDesktopPos);
            SetTitlePosition(titleTextDesktopScale, titleTextDesktopPos, subtitleTextDesktopPos);
        }
        else {
            // mobile mode
            SetMenuIcons(menuIconMobileScale, menuIconMobilePos, mobileFontSize, menuIconTextMobilePos);
            SetMenuButtons(menuButtonMobileScale, menuButtonMobilePos);
            SetTitlePosition(titleTextMobileScale, titleTextMobilePos, subtitleTextMobilePos);
        }
        SetArrowPos();
    }
    
    private void SetMenuIcons(Vector3 iconScale, IReadOnlyList<Vector2> iconPositionArr, float fontSize, IReadOnlyList<Vector2> textPositionArr) {
        for (int i = 0; i < 5; i++) {
            scripts.menuIcon.menuIconOrdering[i].transform.localScale = iconScale;
            scripts.menuIcon.menuIconOrdering[i].transform.localPosition = iconPositionArr[i];
            scripts.menuIcon.menuIconTextOrdering[i].GetComponent<TextMeshProUGUI>().fontSize = fontSize;
            scripts.menuIcon.menuIconTextOrdering[i].transform.localPosition = textPositionArr[i];
        }
    }

    private void SetMenuButtons(Vector3 scale, IReadOnlyList<Vector2> buttonPosArr) {
        Save.LoadGame();
        if (Save.game.newGame) { 
            menuButtons[0].SetActive(false); 
            for (int i = 1; i < menuButtons.Length; i++) {
                menuButtons[i].transform.localScale = scale;
                menuButtons[i].transform.localPosition = buttonPosArr[i-1];
            }
        }
        else { 
            menuButtons[0].SetActive(true); 
            for (int i = 0; i < menuButtons.Length; i++) { 
                menuButtons[i].transform.localScale = scale;
                menuButtons[i].transform.localPosition = buttonPosArr[i];
            }
        }
        foreach (GameObject button in menuButtons) {
            button.GetComponent<MenuButton>().GetComponent<TextMeshProUGUI>().color = Color.white;
        }
    }

    private void SetArrowPos() {
        if (!mobileMode) {
            if (Save.game.newGame) {
                scripts.arrow.transform.localPosition = new Vector2(
                    menuButtons[1].transform.position.x + scripts.arrow.xOffset,
                    menuButtons[1].transform.position.y + scripts.arrow.yOffset
                );
            }
            else {
                scripts.arrow.transform.localPosition = new Vector2(
                    menuButtons[0].transform.position.x + scripts.arrow.xOffset,
                    menuButtons[0].transform.position.y + scripts.arrow.yOffset
                );
            }
        }
        else {
            scripts.arrow.transform.localPosition = new Vector2(1000f, 0);
        }
    }

    private void SetTitlePosition(Vector3 scale, Vector3 titlePos, Vector3 subtitleTextPos) {
        titleText.transform.localScale = scale;
        subtitleText.transform.localScale = scale;
        titleText.transform.localPosition = titlePos;
        subtitleText.transform.localPosition = subtitleTextPos;
    }
}
