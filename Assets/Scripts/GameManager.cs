using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

using UnityEngine.Networking;
using System.Collections;
using LitJson;


public class GameManager : MonoBehaviourPunCallbacks
{
    [DllImport("__Internal")]
    private static extern string GetUserInfo();

    // 방 리스트를 위한
    public Text ListText;
    public Text RoominfoText;
    public Text TalkText;
    public GameObject scanObject;

    // chat function
    public Text[] ChatText;
    public TMP_InputField ChatInput;
    public PhotonView PV;

    public int userPk;
    public int roomPk;
    private string token;

    public GameObject tableobject;
    void Awake()
    {
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", otherPlayer.NickName);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenuScene");

    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        token = GetUserInfo();
        
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            StartCoroutine(DeleteRoom());
        }
        else
        {
            StartCoroutine(LeftRoom());
        }
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoominfoText.text = PhotonNetwork.CurrentRoom.Name + " /" + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    public void TalkAction(GameObject scanObj)
    {
        scanObject = scanObj;
        TalkText.text = "이것의 이름은" + scanObject.name;
        if (scanObject.name == "BaekHouse")
        {
            TalkText.text = "여기는 백준의 집입니다!";
        }
        else if (scanObject.name == "Rock")
        {
            TalkText.text = "빠져봐요 개발의 숲! META STUDY에 오신 여러분들 반갑습니다.";
        }
    }

    public void FocusCanvas(string focus)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
    if (focus == "0")
    {
        WebGLInput.captureAllKeyboardInput = false;
    }
    // disable WebGLInput.captureAllKeyboardInput so elements in web page can handle keabord inputs
    else
    {
        WebGLInput.captureAllKeyboardInput = true;
    }
#endif
    }

    // Chat Send
    public void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    public void ChangeTableName(string name)
    {
        tableobject.GetComponent<OnStudy>().Booker.text = name;
        tableobject.GetComponent<OnStudy>().onRPCtrigger();
        tableobject = null;
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput)
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }


    IEnumerator LeftRoom()
    {
        yield return StartCoroutine(GetUserInfoApi(token));
        yield return StartCoroutine(GetRoomInfoApi());
        yield return StartCoroutine(RoomleaveApi());
    }
    IEnumerator DeleteRoom()
    {
        yield return StartCoroutine(GetRoomInfoApi());
        yield return StartCoroutine(RoomdeleteApi());
    }

    IEnumerator GetUserInfoApi(string token)
    {
        print("방 나갈 때 유저 정보 가져오기");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Get("https://i5b105.p.ssafy.io:8080/api/users/me");
        www.SetRequestHeader("Authorization", "Bearer " + token);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            JsonData ItemData = JsonMapper.ToObject(jsonResult);
            userPk = int.Parse(ItemData["userPk"].ToString());
        }
    }

    IEnumerator GetRoomInfoApi()
    {
        print("방 나갈 때 방 정보 가져오기");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Get("https://i5b105.p.ssafy.io:8080/api/room/roomNumber/" + PhotonNetwork.CurrentRoom.Name);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            JsonData ItemData = JsonMapper.ToObject(jsonResult);
            roomPk =int.Parse(ItemData["roompk"].ToString());
        }
    }

    IEnumerator RoomleaveApi()
    {

        print("userpk = " + userPk);
        print("roompk = " + roomPk);

        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Put("https://i5b105.p.ssafy.io:8080/api/room/leaveroom/" + roomPk + "/" + userPk, "puttest");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("DB에 상태 업데이트");
        }
    }

    IEnumerator RoomdeleteApi()
    {
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Delete("https://i5b105.p.ssafy.io:8080/api/room/" + roomPk);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("DB 삭제");
        }
    }
}