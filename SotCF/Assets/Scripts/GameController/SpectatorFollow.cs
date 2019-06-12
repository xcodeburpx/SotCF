using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorFollow : MonoBehaviour
{

    public GameObject myAvatar;
    public GameObject spectCamera;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindMyAvatar());
    }

    IEnumerator FindMyAvatar()
    {
        yield return new WaitForSeconds(3f);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Avatar");

        //if (players.Length <= 0)
        //    continue;

        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<PhotonView>().IsMine)
            {
                myAvatar = players[i].gameObject;
                transform.position = myAvatar.transform.position + new Vector3(0, 10, 0);
                Vector3 direction = myAvatar.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(myAvatar != null)
        {
            transform.position = myAvatar.transform.position + new Vector3(0, 20, 0);
        }
    }
}
