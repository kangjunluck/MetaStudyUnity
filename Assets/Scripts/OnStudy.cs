using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class OnStudy : MonoBehaviour
{
    public TextMeshProUGUI Booker;
    public PhotonView PV;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void onRPCtrigger()
    {
        PV.RPC("ChangeName", RpcTarget.AllBuffered, Booker.text);
    }
    [PunRPC]
    void ChangeName(string name) => Booker.text = name;
}
