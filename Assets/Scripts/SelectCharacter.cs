using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public Character character;
    void Start()
    {
        
    }

    public void OnChangeCharacter()
    {
        //initialize animation character in LobbyMenuUI 
        GameObject.Find("BigChar").transform.Find(DataMgr.instance.currentCharacter.ToString()).gameObject.SetActive(false);
        DataMgr.instance.currentCharacter = character;
        GameObject.Find("BigChar").transform.Find(character.ToString()).gameObject.SetActive(true);
    }
}
