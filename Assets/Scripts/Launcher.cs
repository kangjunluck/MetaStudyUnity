using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using LitJson;
using System.Runtime.InteropServices;

public class Launcher : MonoBehaviourPunCallbacks
{
    [DllImport("__Internal")]
    private static extern string GetUserInfo();

    [DllImport("__Internal")]
    private static extern void SaveRoompk(string roomPk);

    /*    [DllImport("__Internal")]
        private static extern void SaveRoompk(int num);*/

    string gameVersion = "1";
    public string BASEURL = "https://i5b105.p.ssafy.io:8080/";  

    bool isConnecting;
    public Character character;

    // nickname
    public TMP_InputField nickName;
    public TMP_Text WelcomeText;

    // information about room creation
    public TMP_InputField roomName;

    // public variables for request room info
    public int selectnum;
    public InputField pinNum;
    public InputField enterpinNum;
    private string token;
    public string checkNickname;


    public int roomPk;
    public int userPk;
    public int pin;

    // variables about room list
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;


    #region 방리스트갱신
    public void myListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else
        {
            selectnum = num;
            GameObject.Find("LobbyMenuUI").transform.Find("Pin").gameObject.SetActive(true);
        }
        MyListRenewal();
    }
    public void onclicktojoinroom()
    {
        /*PhotonNetwork.JoinRoom(myList[multiple + selectnum].Name);*/
        /*StartCoroutine(RoomJoinApi(myList[multiple + selectnum].Name, 2));*/
        StartCoroutine(RoomJoin((myList[multiple + selectnum].Name)));
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    void MyListRenewal()
    {

        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    #endregion


    #region connect server
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsConnected)
        {
            GameObject.Find("MainMenuUI").SetActive(false);
            GameObject.Find("Canvas").transform.Find("LobbyMenuUI").gameObject.SetActive(true);
            WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "Welcome!";


            GameObject.Find("BigChar").transform.Find(character.ToString()).gameObject.SetActive(false);
            GameObject.Find("BigChar").transform.Find(DataMgr.instance.currentCharacter.ToString()).gameObject.SetActive(true);
        }
    }
    void Start()
    {
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();


    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            isConnecting = true;
            PhotonNetwork.LocalPlayer.NickName = nickName.text;
            WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "Welcome!";
        }
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) => print("disconnect");


    public void BtnCreateRoom()
    {
        print("BtnCreateRoom press");

        roomName.text = (roomName.text != "" ? roomName.text : "Room" + Random.Range(1, 100));

        StartCoroutine(RoomNameCheck(roomName.text));         

        print(checkNickname);
        // if(checkNickname == "Success")
        // {
        //     print("성공일때");
        //     PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 5 });
        //   GameObject.Find("CreateRoomUI").transform.Find("alertRoomname").gameObject.SetActive(false);
        //   StartCoroutine(RoomCreate());
        // }
        // else{
        //     print("실패일때");
        //     GameObject.Find("CreateRoomUI").transform.Find("alertRoomname").gameObject.SetActive(true);
        // }

        /*RoomCreateApi(roomName.text, pinNum.text);*/

    }
    public void onCreateRoomFailed(short returnCode, string message) => print("room creation FAILED");
    public override void OnCreatedRoom() => print("create room SUCCESS");

    public void JoinRoom() => PhotonNetwork.JoinRoom(roomName.text);
    public override void OnJoinedRoom()
    {
        print("room enter SUCCESS");
        SceneManager.LoadScene("Room for 1");   
    }
    public override void OnJoinRoomFailed(short returnCode, string message) => print("room enter Failed");

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 5 });
    }
    #endregion


    #region API 요청

    IEnumerator RoomCreate()
    {
        yield return StartCoroutine(RoomCreateApi(roomName.text, pinNum.text));
        yield return StartCoroutine(RoomJoinApi(roomName.text));
        yield return StartCoroutine(GetUserInfoApi(token));
        yield return StartCoroutine(SaveUserRoomApi());
    }
    IEnumerator RoomNameCheck(string roomname){
        yield return StartCoroutine(NicknameDup(roomname));
        if(checkNickname == "Success")
        {
            print("성공일때");
            PhotonNetwork.CreateRoom(roomname, new RoomOptions { MaxPlayers = 5 });
            GameObject.Find("CreateRoomUI").transform.Find("alertRoomname").gameObject.SetActive(false);
            StartCoroutine(RoomCreate());
        }
        else{
            print("실패일때");
            GameObject.Find("CreateRoomUI").transform.Find("alertRoomname").gameObject.SetActive(true);
        }
    }
    IEnumerator RoomJoin(string roomname)
    {
        yield return StartCoroutine(RoomJoinApi(roomname));
        if (enterpinNum.text == "" || pin != int.Parse(enterpinNum.text))
        {
            Debug.Log("Null or Dismatch Pin number");
        }
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + selectnum].Name);
            yield return StartCoroutine(GetUserInfoApi(token));
            yield return StartCoroutine(SaveUserRoomApi());
        }
    }
    IEnumerator RoomCreateApi(string roomname, string pinnum)
    {
        print("here is RoomCreateApi");
        WWWForm form = new WWWForm();
        form.AddField("pin", int.Parse(pinnum));
        form.AddField("roomname", roomname);

        UnityWebRequest www = UnityWebRequest.Post(BASEURL+"api/room", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            /*StartCoroutine(RoomJoinApi(roomname, 1));*/
        }
    }


    IEnumerator RoomJoinApi(string roomname)
    {
        print("here in RoomJoinApi");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Get(BASEURL+"api/room/roomNumber/" + roomname);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {

            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);



            JsonData ItemData = JsonMapper.ToObject(jsonResult);
            roomPk = int.Parse(ItemData["roompk"].ToString());
            pin = int.Parse(ItemData["pin"].ToString());

            SaveRoompk(roomPk.ToString());

            token = GetUserInfo();
        }
    }


    IEnumerator GetUserInfoApi(string token)
    {
        print("Here is GetUserInfoApi");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Get(BASEURL+ "api/users/me");
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

            /*StartCoroutine(SaveUserRoomApi());*/
        }
    }

    IEnumerator SaveUserRoomApi()
    {
        print("Here is SaveUserRoomApi");
        WWWForm form = new WWWForm();

        print(PhotonNetwork.LocalPlayer.NickName + "my nickname...");
        form.AddField("roompk", roomPk);
        form.AddField("userpk", userPk);
        form.AddField("username", PhotonNetwork.LocalPlayer.NickName);


        UnityWebRequest www = UnityWebRequest.Post(BASEURL+"api/room/join", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("DB assigned fin");
        }
    }

    IEnumerator NicknameDup(string roomname)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Get(BASEURL+"api/room/roomname/"+roomname);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            JsonData ItemData = JsonMapper.ToObject(jsonResult);
            print(jsonResult+"아이템데이터");
            checkNickname = ItemData["message"].ToString();
            print(checkNickname);
            /*StartCoroutine(SaveUserRoomApi());*/
        }
        

    }

    #endregion


    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("Current Room Name : " + PhotonNetwork.CurrentRoom.Name);
            print("Current Room personnel: " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("Max personnel : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerstr = "Player in this room : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerstr += PhotonNetwork.PlayerList[i].NickName + ',';
            print(playerstr);
        }
        else
        {
            print("Server Connect personnel : " + PhotonNetwork.CountOfPlayers);
            print("The number of rooms : " + PhotonNetwork.CountOfRooms);
            print("The number of whole players : " + PhotonNetwork.CountOfPlayersInRooms);
            print("Is in Lobby? : " + PhotonNetwork.InLobby);
            print("Is connected : " + PhotonNetwork.IsConnected);

        }
    }
}