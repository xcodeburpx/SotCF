using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Material>().color = new Color(
              Random.Range(0, 255),
              Random.Range(0, 255),
              Random.Range(0, 255),
              255
          );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
