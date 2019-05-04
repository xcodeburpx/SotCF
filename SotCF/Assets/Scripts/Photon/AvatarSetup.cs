using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;
    public int characterValue;

    public string myName = "None";
    public bool secondLife = false;

    public int playerHealth;
    public int playerDamage;


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
        }
        //else
        //{
        //    Destroy(myCamera);
        //    Destroy(myAL);
        //}
    }

    void Update()
    {
        if(playerHealth == -1000)
        {
            if (PV.IsMine)
            {
                GameObject.FindGameObjectWithTag("PlayerCanvas").SetActive(false);
                GameObject.FindGameObjectWithTag("Spectator").GetComponent<SpectatorController>().enabled = true;
                GameSetup.GS.healthDisplay.text = "";
                GameSetup.GS.nameDisplay.text = "";
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        characterValue = whichCharacter;
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        //myCharacter.tag = "Avatar";
    }

    [PunRPC]
    void RPC_SetName(string whichName)
    {
        myName = whichName;
    }
}
