using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHandWeapon : MonoBehaviour
{

    public AudioClip shootClip, reloadClip;
    private AudioSource audioManager;
    private GameObject muzzleFlash;

    private Animator anim;

    private string SHOOT = "Shoot";
    private string RELOAD = "Reload";

    private PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        muzzleFlash = transform.Find("MuzzleFlash").gameObject;
        muzzleFlash.SetActive(false);

        audioManager = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void RPC_SoundShoot()
    {
        if(audioManager.clip != shootClip)
        {
            audioManager.clip = shootClip;
        }
        audioManager.Play();
        StartCoroutine(TurnMuzzleFlashOn()); 
    }

    public void Shoot()
    {
        PV.RPC("RPC_SoundShoot", RpcTarget.All);

        anim.SetTrigger(SHOOT);
    }


    IEnumerator TurnMuzzleFlashOn()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    public void Reload()
    {
        StartCoroutine(PlayReloadSound());
        anim.SetTrigger(RELOAD);
    }

    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        if(audioManager.clip != reloadClip)
        {
            audioManager.clip = reloadClip;
        }

        audioManager.Play();
    }
}
