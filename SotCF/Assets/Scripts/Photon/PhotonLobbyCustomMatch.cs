using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobbyCustomMatch : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    // singleton model for all scenes
    public static PhotonLobbyCustomMatch lobby;

    public string roomName;
    public int roomSize;
    public GameObject roomListingPrefab; // Room button with name and number of players
    public Transform roomsPanel;

    public List<RoomInfo> roomListings;

    private void Awake()
    {
        lobby = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomListings = new List<RoomInfo>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        //RemoveRoomListings();
        int tempIndex;
        foreach(RoomInfo room in roomList)
        {
            if(roomListings != null)
            {
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            }
            else
            {
                tempIndex = -1;
            }

            if (tempIndex != -1)
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsPanel.GetChild(tempIndex).gameObject);
            }
            else
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void RemoveRoomListings()
    {
        int i = 0;
        while(roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(i).gameObject);
            i++;
        }
    }

    void ListRoom(RoomInfo room)
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab,roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();
        }
    }

    public void CreateRoom()
    {
        Debug.Log("Creating Room");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize};
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create the room. Probably room exists with the same Name");
        Debug.LogWarning("Message: " + message);
        //CreateRoom();
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;

    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);

    }


    public void JoinLobbyOnClick()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
