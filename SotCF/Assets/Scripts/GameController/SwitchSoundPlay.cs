using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSoundPlay : MonoBehaviour
{
    public AudioClip[] switchSounds;

    public AudioSource audioManager;
    public PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySwitchSound()
    {
        int soundPicker = Random.Range(0, switchSounds.Length);
        if (PV.IsMine)
        {
            PV.RPC("RPC_PlaySoundClip", RpcTarget.All, soundPicker);
        }

    }

    [PunRPC]
    public void RPC_PlaySoundClip(int index)
    {
        audioManager.clip = switchSounds[index];
        audioManager.Play();
    }
}
