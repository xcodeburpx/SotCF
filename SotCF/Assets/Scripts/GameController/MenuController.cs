using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnCharacterPickClicked(int whichCharacter)
    {
        if(PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedCharacter = whichCharacter;
            PlayerPrefs.SetInt("MyCharacter", whichCharacter);
        }
    }
}
