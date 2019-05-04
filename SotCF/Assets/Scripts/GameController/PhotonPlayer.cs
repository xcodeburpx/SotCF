using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{

    public PhotonView PV;
    public GameObject myAvatar;
    public int myTeam;
    public bool isCreated = false;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (myAvatar == null && myTeam != 0 && isCreated == false)
        {
            int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
            if (PV.IsMine)
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                    GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
                //myAvatar.GetComponent<AvatarSetup>().enabled = true;
                myAvatar.GetComponent<PlayerMovement>().enabled = true;
                myAvatar.GetComponent<PlayerMovement>().myCamera.SetActive(true);
                PV.RPC("RPC_IsCreated", RpcTarget.AllBuffered);
            }
        }
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
