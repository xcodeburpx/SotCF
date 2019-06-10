using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathSoundPlay : MonoBehaviour
{

    public AudioClip[] deathSounds;

    public AudioSource audioManager;

    public PhotonView PV;
    
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioSource>();
        int soundPicker = Random.Range(0, deathSounds.Length);
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_PlayDeathClip", RpcTarget.All, soundPicker);
        }
    }

    [PunRPC]
    void RPC_PlayDeathClip(int index)
    {
        StartCoroutine(PlayMusic(index));
    }

    IEnumerator PlayMusic(int index)
    {

        audioManager.clip = deathSounds[index];
        audioManager.Play();
        yield return new WaitForSeconds(7f);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
