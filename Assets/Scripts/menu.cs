using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{

    public void Play()
    {
        Initiate.Fade("VRScene", Color.black, 2f);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quitting...");
            Application.Quit();
        }
    }
}
