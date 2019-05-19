using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{

    public static GameSetup GS;

    public int nextPlayersTeam = 1;
    public Transform[] spawnPoints;

    public Text healthDisplay;
    public Text nameDisplay;
    public Text teamPlayerDisplay;
    public Text winningDisplay;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if(GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }

    private void Start()
    {
        GameSetup.GS.winningDisplay.text = "";
    }

    public void DisconnectPlayer()
    {
        Destroy(PhotonRoomCustomMatch.room.gameObject);
        //Destroy(PhotonRoom.room.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    public void ForceDisconnect()
    {
        Destroy(PhotonRoomCustomMatch.room.gameObject);
        //Destroy(PhotonRoom.room.gameObject);
        StartCoroutine(ForceKickOut());
        Destroy(PhotonRoomCustomMatch.room.gameObject);
    }



    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while(PhotonNetwork.InRoom)
            yield return null;
        Destroy(PhotonRoomCustomMatch.room.gameObject);
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.menuScene);
    }

    IEnumerator ForceKickOut()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.menuScene);
    }

    public void UpdateTeam()
    {
        nextPlayersTeam++;
    }


}
