using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ReSpawn : MonoBehaviourPunCallbacks
{
    public GameObject[] charPrefabs;
    public string pick;
    void Start()
    {
        Transform[] points = GameObject.Find("SpawnPointsGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        if (DataMgr.instance.currentCharacter.ToString() == "Slime")
        {
            pick = "Slime";
        }
        else if (DataMgr.instance.currentCharacter.ToString() == "PinkMoo")
        {
            pick = "PinkMoo";
        }
        else
        {
            pick = "AngryPig";
        }
        Vector3 position = points[idx].position;
        position[2] = 0;
        points[idx].position = position;
        PhotonNetwork.Instantiate(pick, points[idx].position, Quaternion.identity);
    }

}
