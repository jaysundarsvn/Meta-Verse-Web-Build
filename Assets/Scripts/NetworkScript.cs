using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using TMPro;

public class NetworkScript : MonoBehaviourPunCallbacks
{
    public TMP_InputField createCodeInput;
    public TMP_InputField joinCodeInput;
    public TMP_InputField PlayerNameInput;
    public TMP_InputField ChatInput;
    public static TMP_InputField CheckChatBox;
    public TMP_Text joinCodeDisplay;
    public GameObject MainMenuCanvas;
    public GameObject GameCanvas;
    public GameObject LoadingScreen;
    public GameObject Screen0;
    public GameObject PlayerPrefab;
    public Transform StartPosition;
    public static string message = "";
    public static bool IsChatBubble = false;
    public static bool IsInRoom = false;


    public static string PlayerName = "player";

    string joinCode;
    // Start is called before the first frame update
    void Start()
    {
        CheckChatBox = ChatInput;
        ChatInput.gameObject.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        MainMenuCanvas.SetActive(true);
        GameCanvas.SetActive(false);
        LoadingScreen.SetActive(true);
        Screen0.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        LoadingScreen.SetActive(false);
        Screen0.SetActive(true);
    }

    // Update is called once per frame
     void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && IsInRoom)
        {
            ChatInput.gameObject.SetActive(true);
            ChatInput.Select();
            ChatInput.ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.E) && IsInRoom && !ChatInput.isFocused)
        {
            LeaveRoom();
        }

    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createCodeInput.text))
        {
            Debug.Log("Join code cannot be empty.");
            return;
        }

        LoadingScreen.SetActive(true);
        Screen0.SetActive(false);
        joinCode = createCodeInput.text;
        PhotonNetwork.CreateRoom(joinCode);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(createCodeInput.text))
        {
            Debug.Log("Join code cannot be empty.");
            return;
        }

        LoadingScreen.SetActive(true);
        Screen0.SetActive(false);
        joinCode = joinCodeInput.text;
        PhotonNetwork.JoinRoom(joinCode);
    }

    public override void OnJoinedRoom()
    {
        IsInRoom = true;
        LoadingScreen.SetActive(false);
        MainMenuCanvas.SetActive(false);
        GameCanvas.SetActive(true);
        joinCodeDisplay.text = "Join Code: " + joinCode;
        PhotonNetwork.Instantiate(PlayerPrefab.name, StartPosition.position, StartPosition.rotation);
        PlayerName = PlayerNameInput.text;
    }

    public void GetMessage()
    {
        message = ChatInput.text;
        IsChatBubble = true;
        ChatInput.text = "";
        ChatInput.gameObject.SetActive(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        GameCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        LoadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        IsInRoom = false;
        LoadingScreen.SetActive(false);
        Screen0.SetActive(true); 
    }
}
