using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickToggle : MonoBehaviour {
    Scripts scripts;
    void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    void OnMouseDown() {
        scripts.menuIcon.PlayerPrefSetter(name, GetComponent<SpriteRenderer>());
    }
}
