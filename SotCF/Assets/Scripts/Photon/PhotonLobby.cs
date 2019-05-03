using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    // singleton model for all scenes
    public static PhotonLobby lobby;

    public GameObject battleButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);
    }


    public void OnBattleButtonClicked()
    {
        Debug.Log("Battle Button Clicked");
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to connect to room. Probably no room existing");
        Debug.LogWarning("Message: " + message);
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating Room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("Room-" + randomRoomName.ToString(), roomOps);
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create the room. Probably room exists with the same Name");
        Debug.LogWarning("Message: " + message);
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Button Clicked");
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
