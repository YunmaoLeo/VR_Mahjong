using System;
using UnityEngine;

public class ActualPlatformController : MonoBehaviour
{
    [SerializeField] private Transform GrabblePlatform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        var parentRotationY = GrabblePlatform.localRotation.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(0, -parentRotationY, 0);
    }
}
