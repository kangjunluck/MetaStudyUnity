using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.InteropServices;
using System;

public class Bullet : MonoBehaviourPunCallbacks
{
    [DllImport("__Internal")]
    private static extern void HitNicknametoVue(String yourname, string myname);

    public PhotonView PV;
    int dir;
    Vector3 dirVec;
    void Start() 
    {
        Destroy(gameObject, 1.5f);
    }

    void Update()
    {
        if (dir == 0) dirVec = Vector3.right;
        else if (dir == 1) dirVec = Vector3.left;
        else if (dir == 2) dirVec = Vector3.up;
        else dirVec = Vector3.down;
        transform.Translate(dirVec * 10 * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PV.IsMine && collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine)
        {

            print(PV.Owner.NickName);
            print(collision.GetComponent<PlayerMov>().PV.Owner.NickName);

            HitNicknametoVue(PV.Owner.NickName, collision.GetComponent<PlayerMov>().PV.Owner.NickName);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        
    }

    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
