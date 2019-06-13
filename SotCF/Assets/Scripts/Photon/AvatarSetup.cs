using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;
    public int characterValue;

    public string myName = "None";
    public GameObject mySuperior = null;
    //public bool mySuperiorExists = false;
    public bool secondLife = false;

    public int playerHealth;
    public int playerDamage;

    public GameObject spectator;
    public string[] gravesNames;


    //public Camera myCamera;
    //public AudioListener myAL;
    //public GameObject myCamera;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_SetName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
            Vector3 colorParams = new Vector3(Random.value, Random.value, Random.value);
            PV.RPC("RPC_SetSuperior", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            PV.RPC("RPC_SetColor", RpcTarget.AllBuffered, colorParams.x, colorParams.y, colorParams.z);
        }
        //else
        //{
        //    Destroy(myCamera);
        //    Destroy(myAL);
        //}
    }

    void Update()
    {
        if(checkDeath())
        {
            if (PV.IsMine)
            {
                int gravePicker = Random.Range(0, gravesNames.Length);
                //GameObject.FindGameObjectWithTag("PlayerCanvas").SetActive(false);
                //GameObject.FindGameObjectWithTag("Spectator").GetComponent<SpectatorController>().enabled = true;
                //GameObject.FindGameObjectWithTag("Spectator").GetComponent<SpectatorMouseLock>().enabled = true;
                //GameObject.FindGameObjectWithTag("Spectator").transform.GetChild(0).gameObject.SetActive(true);
                var playerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas");
                var spectatior = GameObject.FindGameObjectWithTag("Spectator");
                spectatior.GetComponent<SpectatorController>().enabled = true;
                spectatior.GetComponent<SpectatorMouseLock>().enabled = true;
                spectatior.transform.GetChild(0).gameObject.SetActive(true);
                //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = true;
                playerCanvas.transform.Find("Image").gameObject.SetActive(false);
                GameSetup.GS.healthDisplay.text = "";
                GameSetup.GS.nameDisplay.text = "";
                GameSetup.GS.teamPlayerDisplay.text = "Spectator Mode";
                GameSetup.GS.winningDisplay.text = "";

                PhotonNetwork.Instantiate(Path.Combine("Misc","PhotonPrefabs", "Graves", gravesNames[gravePicker]),transform.position, transform.rotation,0);
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
    public bool checkDeath()
    {
        return playerHealth <= -1000
            || transform.position.y <= PhotonRoomCustomMatch.room.waterLevel
            || mySuperior == null
            || mySuperior.GetComponent<AvatarSetup>().playerHealth <= -1000
            || mySuperior.GetComponent<AvatarSetup>().secondLife;
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        characterValue = whichCharacter;
        //myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        //myCharacter.tag = "Avatar";
    }

    [PunRPC]
    void RPC_SetName(string whichName)
    {
        myName = whichName;
    }
    [PunRPC]
    void RPC_SetSuperior(string whichSuperior)
    {
        mySuperior = gameObject;
    }

    [PunRPC]
    void RPC_SetColor(float x, float y, float z)
    {
        Color newColor = new Color(x, y, z, 1.0f);
        GetComponent<Renderer>().material.color = newColor;
    }
}
