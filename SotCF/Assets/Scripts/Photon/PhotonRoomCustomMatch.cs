using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonRoomCustomMatch : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public static PhotonRoomCustomMatch room;

    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    // Player info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    // Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;


    public GameObject lobbyGO;
    public GameObject roomGO;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;


    private void Awake()
    {
        if(PhotonRoomCustomMatch.room == null)
        {
            PhotonRoomCustomMatch.room = this;
        }
        else
        {
            if(PhotonRoomCustomMatch.room != this)
            {
                Destroy(PhotonRoomCustomMatch.room.gameObject);
                PhotonRoomCustomMatch.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;

    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if(playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if(readyToStart)
                {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                Debug.Log("Display time to start the the players: " + timeToStart);
                if(timeToStart <= 0f)
                {
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We Are NOW connected to the ROOM");

        lobbyGO.SetActive(false);
        roomGO.SetActive(true);

        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }

        ClearPlayersListings();
        ListPlayers();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        if(MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log("Display players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if(playersInRoom > 1)
            {
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if(!PhotonNetwork.IsMasterClient)
                {
                    return;
                }
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        //else
        //{
        //    StartGame();
        //}
    }

    void ClearPlayersListings()
    {
        for(int i = playersPanel.childCount -1; i >= 0; i--)
        {
            Destroy(playersPanel.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject tempListing = Instantiate(playerListingPrefab ,playersPanel);
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has ENTERED the ROOM");

        ClearPlayersListings();
        ListPlayers();


        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if(MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log("Display players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if(playersInRoom > 1)
            {
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;

                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }


    public void StartGame()
    {
        isGameLoaded = true;
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        if(MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }


    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }


    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;

            if(MultiplayerSettings.multiplayerSettings.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }


    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity,0);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has LEFT THE GAME");
        playersInGame--;
        //ClearPlayersListings();
        //ListPlayers();
    }

}
