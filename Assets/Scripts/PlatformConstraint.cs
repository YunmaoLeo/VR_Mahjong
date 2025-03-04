using System;
using UnityEngine;

public class PlatformConstraint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.localPosition = new Vector3();
        var currentY = transform.localRotation.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(0,currentY, 0);
    }
}
