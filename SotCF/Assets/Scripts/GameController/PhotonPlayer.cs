using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonPlayer : MonoBehaviour
{

    public PhotonView PV;
    public GameObject myAvatar;
    public int myTeam;
    public bool isCreated = false;


    public bool ifGameWon = true;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
        StartCoroutine(CheckDelay());

    }

    // Update is called once per frame
    void Update()
    {
        if (myAvatar == null && myTeam != 0 && isCreated == false)
        {
            int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
            Debug.Log("Spawn Point Number: " + spawnPicker);
            if (PV.IsMine)
            {
                // Random spot in spawnPoint
                var ran = Random.insideUnitSphere * PhotonRoomCustomMatch.room.radiusSpawn;
                ran.y = 0.0f;

                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                    GameSetup.GS.spawnPoints[spawnPicker].position+ran, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
                //myAvatar.GetComponent<AvatarSetup>().enabled = true;
                myAvatar.GetComponent<PlayerMovement>().enabled = true;
                myAvatar.GetComponent<PlayerMovement>().myCamera.SetActive(true);
                myAvatar.transform.Find("FPS View").GetComponent<SimpleSmoothMouseLook>().enabled = true;
                PV.RPC("RPC_IsCreated", RpcTarget.AllBuffered);
            }
        }

        CheckGame();
    }

    IEnumerator CheckDelay()
    {
        yield return new WaitForSeconds(10f);
        ifGameWon = false;
    }

    void CheckGame()
    {
        if (PhotonNetwork.IsMasterClient && !ifGameWon)
        {
            // Condition Parameters
            string winner = "None";
            int length;

            // If player exists - grab first name
            GameObject[] players = GameObject.FindGameObjectsWithTag("Avatar");

            //if (players.Length <= 0)
            //    continue;

            for (int i = 0; i < players.Length; i++)
            {
                winner = players[0].GetComponent<AvatarSetup>().myName;
                break;
            }

            // Look for occurence of first name
            for (length = 0; length < players.Length; length++)
            {
                if (players[length].GetComponent<AvatarSetup>().myName != winner)
                    break;
            }
            //Debug.Log("Winner name and length : " + winner + " | " + length);
            //Debug.Log("Length of List : " + players.Length);

            if (length == players.Length && winner != "None")
            {
                Debug.Log("ENTERED ForceMenu Coroutine()");
                PV.RPC("RPC_ForceKick", RpcTarget.All, winner);
                ifGameWon = true;
            }
        }
    }
    [PunRPC]
    void RPC_ForceKick(string winner)
    {
        ifGameWon = true;
        StartCoroutine(WinningScreen(winner));
        StartCoroutine(ForceMenu());
    }

    IEnumerator WinningScreen(string winner)
    {
        GameSetup.GS.teamPlayerDisplay.text = "";
        GameSetup.GS.winningDisplay.text = winner + " has won the game!!";
        yield return new WaitForSeconds(3f);
        GameSetup.GS.winningDisplay.text = "";
    }

    IEnumerator ForceMenu()
    {
        Debug.Log("Game has been won! Force Reload to menu");
        yield return new WaitForSeconds(4f);
        //GameSetup.GS.DisconnectPlayer();
        GameSetup.GS.ForceDisconnect();
    }

    [PunRPC]
    void RPC_GetTeam()
    {
        myTeam = GameSetup.GS.nextPlayersTeam;
        GameSetup.GS.UpdateTeam();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    }

    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        myTeam = whichTeam;
    }

    [PunRPC]
    void RPC_IsCreated()
    {
        isCreated = true;
    }

}
