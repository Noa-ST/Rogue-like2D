using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterScriptObject characterData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    public static CharacterScriptObject GetData()
    {
        return instance.characterData;
    }

    public void SelectCharacter(CharacterScriptObject character)
    {
        characterData = character;
    }

    public void destroySingleTon()
    {
        instance = null;
        Destroy(gameObject);
    }
}
