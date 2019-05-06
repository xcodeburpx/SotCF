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
    private Text nameDisplay;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<AvatarSetup>();
        healthDisplay = GameSetup.GS.healthDisplay;
        nameDisplay = GameSetup.GS.nameDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PV.IsMine)
        { 
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            RPC_Shooting();
            PV.RPC("RPC_ShootSound", RpcTarget.AllBuffered);
        }

        healthDisplay.text = avatarSetup.playerHealth.ToString();
        nameDisplay.text = avatarSetup.myName;
    }

    void RPC_Shooting()
    {

        RaycastHit hit;
        Ray ray = myCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log("Did Hit");
            Debug.Log("The Tag: " + hit.transform.tag);
            if (hit.transform.tag == "Avatar" && hit.transform.GetComponent<AvatarSetup>().myName != GetComponent<AvatarSetup>().myName)
            {
                hit.transform.GetComponent<PhotonView>().RPC("RPC_ApplyDamage", RpcTarget.AllBuffered, GetComponent<AvatarSetup>().playerDamage, GetComponent<AvatarSetup>().myName);

            }
        }
        else
        {
            Debug.Log("Did not Hit");
        }
    }

    [PunRPC]
    void RPC_ShootSound()
    {
        var audioclip = transform.GetChild(1).GetComponent<AudioSource>();
        if (audioclip.isPlaying)
        {
            audioclip.Stop();
        }
        audioclip.Play();
    }

    IEnumerator CreateParticle(RaycastHit hit)
    {
        GameObject particle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Particle"), hit.point, Quaternion.LookRotation(hit.normal), 0) as GameObject;
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(particle);
    }

    [PunRPC]
    void RPC_ApplyDamage(int dmg, string whichName)
    {
        GetComponent<AvatarSetup>().playerHealth -= dmg;
        if(GetComponent<AvatarSetup>().playerHealth <= 0)
        {
            if (GetComponent<AvatarSetup>().secondLife == false)
            {
                GetComponent<AvatarSetup>().playerHealth = 100;
                GetComponent<AvatarSetup>().myName = whichName;
                GetComponent<AvatarSetup>().secondLife = true;
            }

            else
            {
                GetComponent<AvatarSetup>().playerHealth = -1000;
            }
        }
        Debug.Log("I Have been Hit!");
    }

    [PunRPC]
    void RPC_ChangeName(string whichName)
    {
        GetComponent<AvatarSetup>().myName = whichName;
    }
}
