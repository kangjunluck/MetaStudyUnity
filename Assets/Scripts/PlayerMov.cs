using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class PlayerMov : MonoBehaviourPunCallbacks, IPunObservable
{
    [DllImport("__Internal")]
    private static extern void NicknametoVue(String myname, String yourname);
    [DllImport("__Internal")]
    private static extern void HitNicknametoVue(String yourname, string myname);
    [DllImport("__Internal")]
    private static extern void GoBaekHouse();

    [DllImport("__Internal")]
    private static extern void GoSWEAHouse();

    [DllImport("__Internal")]
    private static extern void ShowBoard();

    [DllImport("__Internal")]
    private static extern void ShowTime();

    private float h, v;
    public Animator AN;

    GameManager manager;
    private Transform tr;
    Vector3 dirVec;
    int way;
    GameObject scanObject;
    GameObject playerObject;

    public float speed;
    private Vector3 currPos;

    public PhotonView PV;
    public SpriteRenderer SR;
    public Rigidbody2D RB;

    public TextMeshProUGUI CharName;
    void Start()
    {
        tr = GetComponent<Transform>();

        if (photonView.IsMine)
        {
            Camera.main.GetComponent<CameraController>().m_Player = tr;
        }
        CharName.text = photonView.Owner.NickName;

    }
    void Update ()
    {
        if (PV.IsMine)
        {
            // 이동
            float xaxis = Input.GetAxisRaw("Horizontal");
            float yaxis = Input.GetAxisRaw("Vertical");
            RB.velocity = new Vector3(4*xaxis, 4*yaxis,0);

            if (xaxis != 0 || yaxis != 0)
            {
                AN.SetBool("walk", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, xaxis);
            }
            else
            {
                AN.SetBool("walk", false);
            }
        }
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            Vector3 moveVec = new Vector3(h, v, 0);

            //Direction
            if (v == 1)
            {
                dirVec = Vector3.up;
                way = 2;
            }
            else if (v == -1)
            {
                dirVec = Vector3.down;
                way = 3;
            }
            else if (h == -1)
            {
                dirVec = Vector3.left;
                way = 1;
            }
            else if (h == 1)
            {
                dirVec = Vector3.right;
                way = 0;
            }

            //ray find object
            Debug.DrawRay(tr.position, dirVec * 1.0f, new Color(0, 1, 0));

            RaycastHit2D[] rayHits = Physics2D.RaycastAll(tr.position, dirVec, 1.0f, LayerMask.GetMask("player"));
            for (int i = 0; i < rayHits.Length; i++)
            {
                RaycastHit2D hit = rayHits[i];
                if (hit.collider.gameObject.GetComponent<PlayerMov>().PV.Owner.NickName != PV.Owner.NickName)
                {
                    playerObject = hit.collider.gameObject;
                    break;
                }
                else
                {
                    playerObject = null;
                }
            }
            //Scan player
            if (Input.GetButtonDown("Jump") && playerObject != null)
            {
                print(PV.Owner.NickName);
                NicknametoVue(PV.Owner.NickName, PV.Owner.NickName);
                print(tr.position);
                PhotonNetwork.Instantiate("Bullet", tr.position, Quaternion.identity)
                    .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, way);

                Debug.Log("connect with :" + playerObject.GetComponent<PlayerMov>().PV.Owner.NickName);
            }


            RaycastHit2D rayHit = Physics2D.Raycast(tr.position, dirVec, 1.0f, LayerMask.GetMask("object"));
            if (rayHit.collider != null)
            {
                scanObject = rayHit.collider.gameObject;
            }
            else
                scanObject = null;

            //Scan object
            if (Input.GetKeyDown(KeyCode.C) && scanObject != null)
            {
                Debug.Log("this is :" + scanObject.name);
                if (scanObject.name == "BaekHouse")
                {
                    Debug.Log("백준");
                    GoBaekHouse();
                }
                else if (scanObject.name == "SweaHouse")
                {
                    Debug.Log("SWEA");
                    GoSWEAHouse();
                }
                else if (scanObject.name == "Board")
                {
                    Debug.Log("Board");
                    ShowBoard();
                }
                else if (scanObject.name == "Timer")
                {
                    Debug.Log("Timer");
                    ShowTime();
                }
                else if (scanObject.name == "Table")
                {
                    if (scanObject.GetComponent<OnStudy>().Booker.text == "")
                    {
                        NicknametoVue(PV.Owner.NickName, PV.Owner.NickName);

                        GameObject.Find("GameManager").GetComponent<GameManager>().tableobject = scanObject;
                        /*scanObject.GetComponent<OnStudy>().Booker.text = PV.Owner.NickName;
                        scanObject.GetComponent<OnStudy>().onRPCtrigger();*/
                    }
                    else
                    {
                        Debug.Log("이미 스터디가 있습니다.");
                        HitNicknametoVue(scanObject.GetComponent<OnStudy>().Booker.text, PV.Owner.NickName);
                    }
                }
                else
                {
                    Debug.Log("여기야?");
                }

            }
        }
        else if ((tr.position - currPos).sqrMagnitude >= 100) tr.position = currPos;
        else
        {
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * speed);
        }
    }
    [PunRPC]
    void FlipXRPC(float xaxis) => SR.flipX = xaxis == 1;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
        }
    }

}
