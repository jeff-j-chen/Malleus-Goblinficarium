using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] controllers;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] public TextMeshProUGUI woundGUIElement;
    [SerializeField] public TextMeshProUGUI staminaCounter;
    [SerializeField] public TextMeshProUGUI target;
    [SerializeField] public TextMeshProUGUI targetInfo;
    [SerializeField] public List<GameObject> inventory = new List<GameObject>();
    [SerializeField] public TextMeshProUGUI identifier;
    [SerializeField] private GameObject statusEffectIcon;
    [SerializeField] public string[] statusEffectNames = new string[] { "dodge", "leech", "fury", "haste", "courage" };
    [SerializeField] public string[] statusEffectDescs = new string[] { "if you strike first, ignore all damage", "cure the same wound as inflicted", "all picked die turn yellow", "pick 3 dice, enemy gets the rest", "keep 1 of your die till next round" };
    [SerializeField] public Sprite[] statusEffectSprites;
    private List<GameObject> statusEffectList = new List<GameObject>();
    public List<string> availableTargets = new List<string>();
    public List<string> woundList = new List<string>();
    public bool isDead;
    public bool cancelMove = false;
    public float hintTimer;
    public Coroutine coroutine = null;
    public Dictionary<string, int> stats = new Dictionary<string, int>()
    {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    public Dictionary<string, int> potionStats = new Dictionary<string, int>()
    {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    public int charNum = 0;
    public int stamina = 3;
    public int targetIndex = 0;
    private Scripts scripts;
    public bool isFurious = false;
    public bool isDodgy = false;
    public bool isHasty = false;
    public bool isBloodthirsty = false;
    public bool isCourageous;

    private void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        identifier.text = "You";
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = icons[charNum];
        GetComponent<Animator>().runtimeAnimatorController = controllers[charNum];
        staminaCounter.text = stamina.ToString();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") < 0f  && !scripts.turnManager.isMoving))
        {
            scripts.turnManager.SetAvailableTargetsOf("player");
            if (targetIndex < scripts.player.availableTargets.Count)
            {
                if (hintTimer > 0.05f)
                {
                    hintTimer += 0.1f;
                }
                targetIndex++;
                scripts.turnManager.SetTargetOf("player");
            }
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) && !scripts.turnManager.isMoving) || (Input.GetAxis("Mouse ScrollWheel") > 0f  && !scripts.turnManager.isMoving))
        {
            if (targetIndex > 0)
            {  
                if (hintTimer > 0.05f)
                {
                    hintTimer += 0.1f;
                }
                targetIndex--;
                scripts.turnManager.SetTargetOf("player");
            }
        }
    }

    public void UseWeapon()
    {
        if (scripts.statSummoner.SumOfStat("green", "player") >= 7 && target.text != "face" && hintTimer <= 0.05 && PlayerPrefs.GetString("hints") == "on")
        {
            coroutine = StartCoroutine(HintFace());
        }
        else if (scripts.player.woundList.Contains(target.text.Substring(1)))
        {
            coroutine = StartCoroutine(HintTargetingWounded());
        }
        else if (hintTimer > 0.05f)
        {
            // player hits enter again, so immediately start the round
            StopCoroutine(coroutine);
            coroutine = null;
            hintTimer = 0f;
            scripts.turnManager.RoundOne();
        }
        else
        {
            scripts.turnManager.RoundOne();
        }
    }

    private IEnumerator HintFace()
    {
        scripts.turnManager.SetStatusText("note: you can aim to the face");
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.1f)
        {
            yield return scripts.delays[0.1f];
        }
        scripts.turnManager.RoundOne();
    }

    public IEnumerator HintTargetingWounded()
    {
        scripts.turnManager.SetStatusText("note: you are targeting a wounded body part");
        for (hintTimer = 3f; hintTimer > 0; hintTimer -= 0.1f)
        {
            yield return scripts.delays[0.1f];
        }
        scripts.turnManager.RoundOne();
    }

    public Sprite GetDeathSprite()
    {
        return deathSprites[charNum];
    }

    public void SetPlayerPositionAfterDeath()
    {
        if (charNum == 0)
        {
            MoveBy(-0.1266667f, 0.6633333f);
        }
        else if (charNum == 1)
        {
            MoveBy(0f, 0.6633333f);
        }
        else if (charNum == 2)
        {
            MoveBy(0f, 0.6633333f);
        }
        else if (charNum == 3)
        {
            MoveBy(0.0566667f, 0.6633333f);
        }
        else { print("bad"); }
    }

    private void MoveBy(float x, float y)
    {
        transform.position = new Vector2(transform.position.x - x, transform.position.y - y);
        transform.GetChild(0).transform.position = new Vector2(transform.GetChild(0).transform.position.x + x, transform.GetChild(0).transform.position.y + y);
    }

    public void SetPlayerStatusEffect(string statusEffect, string onOrOff)
    {
        if (onOrOff == "on")
        {
            identifier.text = ":";
            GameObject icon = Instantiate(statusEffectIcon, new Vector2(-10.25f + 1f * statusEffectList.Count, 3.333f), Quaternion.identity);
            icon.GetComponent<SpriteRenderer>().sprite = statusEffectSprites[Array.IndexOf(statusEffectNames, statusEffect)];
            statusEffectList.Add(icon);
        }
        if (onOrOff == "off")
        {
            IEnumerable<GameObject> matchingIcons = from icon in statusEffectList where icon.GetComponent<SpriteRenderer>().sprite.name == statusEffect select icon;
            foreach (GameObject icon in matchingIcons.ToList()) 
            { 
                statusEffectList.Remove(icon);
                Destroy(icon); 
            }
            if (statusEffectList.Count <= 0) { identifier.text = "You"; }
        }
    }
}