using System;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{ 
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnterForButton: " + other.gameObject.name);
        if (other.gameObject.CompareTag("ButtonTrigger"))
        {
            IntroSceneManager.Instance.OnStartIntroVideo();
        }
    }

    void Update()
    {
        
    }
}
