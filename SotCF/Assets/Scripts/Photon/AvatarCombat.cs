using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCombat : MonoBehaviour
{

    private PhotonView PV;
    private AvatarSetup avatarSetup;
    public Transform rayOrigin;
    public Camera myCam;

    private Text healthDisplay;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<AvatarSetup>();
        healthDisplay = GameSetup.GS.healthDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PV.IsMine)
        { 
            return;
        }
        if (Input.GetMouseButton(0))
        {
            RPC_Shooting();
        }

        healthDisplay.text = avatarSetup.playerHealth.ToString();
    }

    void RPC_Shooting()
    {
        //if (PV.IsMine)
        //{
            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            //if (Physics.Raycast(rayOrigin.position, rayOrigin.TransformDirection(Vector3.forward), out hit, 1000f))

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.Log("Did Hit");
                Debug.Log("The Tag: " + hit.transform.tag);
                if (hit.transform.tag == "Avatar")
                {
                    hit.transform.GetComponent<PhotonView>().RPC("RPC_ApplyDamage", RpcTarget.AllBuffered, GetComponent<AvatarSetup>().playerDamage);
                }
                //StartCoroutine(CreateParticle(hit));
            }
            else
            {
                Debug.Log("Did not Hit");
            }
        //}
    }

    IEnumerator CreateParticle(RaycastHit hit)
    {
        GameObject particle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Particle"), hit.point, Quaternion.LookRotation(hit.normal), 0) as GameObject;
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(particle);
    }

    [PunRPC]
    void RPC_ApplyDamage(int dmg)
    {
        GetComponent<AvatarSetup>().playerHealth -= dmg;
        if(GetComponent<AvatarSetup>().playerHealth <= 0)
        {
            GetComponent<AvatarSetup>().playerHealth = 100;
        }
        Debug.Log("I Have been Hit!");
    }
}
