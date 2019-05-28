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

    public AudioClip[] hitSounds;

    private Text healthDisplay;
    private Text nameDisplay;
    private Text teamPlayerDisplay;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<AvatarSetup>();
        healthDisplay = GameSetup.GS.healthDisplay;
        nameDisplay = GameSetup.GS.nameDisplay;
        teamPlayerDisplay = GameSetup.GS.teamPlayerDisplay;
        teamPlayerDisplay.text = "";
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
            PV.RPC("RPC_ShootSound", RpcTarget.All);
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
            else
            {
                PV.RPC("RPC_HitSound", RpcTarget.All, hit.point);
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
            var audioclip = transform.GetChild(2).GetComponent<AudioSource>();
            if (audioclip.isPlaying)
            {
                audioclip.Stop();
            }
            audioclip.Play();
        
    }

    [PunRPC]
    void RPC_HitSound(Vector3 position)
    {
        int soundPicker = Random.Range(1, hitSounds.Length);
        AudioSource.PlayClipAtPoint(hitSounds[soundPicker], position, 1.0f);
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
        if (PV.IsMine)
        {
            AudioSource.PlayClipAtPoint(hitSounds[0], transform.position);
        }
        GetComponent<AvatarSetup>().playerHealth -= dmg;
        if(GetComponent<AvatarSetup>().playerHealth <= 0)
        {
            if (GetComponent<AvatarSetup>().secondLife == false)
            {
                GetComponent<AvatarSetup>().playerHealth = 100;
                GetComponent<AvatarSetup>().myName = whichName;
                GetComponent<AvatarSetup>().secondLife = true;
                GameObject[] superiors = GameObject.FindGameObjectsWithTag("Avatar");
                for(int i=0; i < superiors.Length; i++)
                {
                    if (superiors[i].GetComponent<PhotonView>().Owner.NickName == whichName)
                    {
                        GetComponent<AvatarSetup>().mySuperior = superiors[i];
                        transform.GetChild(2).GetComponent<Material>().color = superiors[i].transform.GetChild(2).GetComponent<Material>().color;
                        break;
                    }
                    
                }
                //GetComponent<AvatarSetup>().mySuperiorExists = true;
                StartCoroutine(teamChange(whichName));
                transform.localScale *= 0.5f;

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

    IEnumerator teamChange(string whichName)
    {
        if (PV.IsMine)
        {
            teamPlayerDisplay.text = "You have been teamed up with " + whichName;
            yield return new WaitForSeconds(3f);
            teamPlayerDisplay.text = "";
        }
    }
}
