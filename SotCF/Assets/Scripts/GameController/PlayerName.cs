using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    public Text nameTag;

    [PunRPC]
    public void RPC_updateName(string name)
    {
        nameTag.text = name;
    }
}
