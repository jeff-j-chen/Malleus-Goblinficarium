using UnityEngine;
public class OnClickToggle : MonoBehaviour {
    private Scripts scripts;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
    }

    private void OnMouseDown() {
        if (gameObject.name == "tumblr") {
            Application.OpenURL("https://ampersandbear.tumblr.com/"); 
            scripts.soundManager.PlayClip("click0");
        }
        else if (gameObject.name == "twitter") { 
            Application.OpenURL("https://twitter.com/ampersandbear"); 
            scripts.soundManager.PlayClip("click0");
        }
        else { scripts.menuIcon.PlayerPrefSetter(name, GetComponent<SpriteRenderer>()); }
        
    }
}
