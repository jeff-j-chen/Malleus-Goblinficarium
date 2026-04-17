using UnityEngine;
public class OnClickToggle : MonoBehaviour {
    private Scripts s;

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
    }

    private void OnMouseDown() {
        if (gameObject.name == "tumblr") {
            Application.OpenURL("https://ampersandbear.tumblr.com/"); 
            s.soundManager.PlayClip("click0");
        }
        else if (gameObject.name == "twitter") { 
            Application.OpenURL("https://twitter.com/ampersandbear"); 
            s.soundManager.PlayClip("click0");
        }
        else { s.menuIcon.PlayerPrefSetter(name, GetComponent<SpriteRenderer>()); }
        
    }
}
