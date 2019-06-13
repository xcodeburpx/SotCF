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
        GameSetup.GS.teamPlayerDisplay.text = "";
        GameSetup.GS.winningDisplay.text = "";

        StartCoroutine(MusicLevelObjects());
    }

    IEnumerator MusicLevelObjects()
    {
        yield return new WaitForSeconds(0.5f);
        AudioSource[] audioHandlers = GameObject.FindObjectsOfType<AudioSource>();

        if (PlayerPrefs.HasKey("MusicLevel")) {
            for (int i = 0; i < audioHandlers.Length; i++)
            {
                audioHandlers[i].volume = PlayerPrefs.GetFloat("MusicLevel");
            }
        }

    }

    public void DisconnectPlayer()
    {
        Destroy(PhotonRoomCustomMatch.room.gameObject);
        //Destroy(PhotonRoom.room.gameObject);
        StartCoroutine(DisconnectAndLoad());
        Destroy(PhotonRoomCustomMatch.room.gameObject);
    }

    public void ForceDisconnect()
    {
        //Destroy(PhotonRoomCustomMatch.room.gameObject);
        Destroy(PhotonRoomCustomMatch.room.gameObject);
        StartCoroutine(ForceKickOut());
        //StartCoroutine(DisconnectAndLoad());
        //Destroy(PhotonRoomCustomMatch.room.gameObject);
    }



    IEnumerator DisconnectAndLoad()
    {
        if (PhotonNetwork.NetworkClientState != Photon.Realtime.ClientState.Leaving)
        {
            PhotonNetwork.LeaveRoom();
            //Destroy(PhotonRoomCustomMatch.room.gameObject);
        }
        while (PhotonNetwork.InRoom)
            yield return null;
        //Destroy(PhotonRoomCustomMatch.room.gameObject);
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
