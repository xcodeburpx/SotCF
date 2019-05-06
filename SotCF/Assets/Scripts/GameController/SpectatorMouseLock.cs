using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMouseLock : MonoBehaviour
{

    void UnlockMouse()
    {
        if (Input.GetKeyDown("1"))
        {
            GetComponent<SpectatorController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown("2"))
        {
            GetComponent<SpectatorController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        UnlockMouse();
    }
}
