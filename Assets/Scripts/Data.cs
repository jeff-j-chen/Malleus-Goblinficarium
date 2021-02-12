using UnityEngine;

[System.Serializable]
public class Data {
    public int curCharNum;
    public int newCharNum;

    public Data(Scripts scripts) { 
        newCharNum = scripts.characterSelector.selectionNum;
    }
}
