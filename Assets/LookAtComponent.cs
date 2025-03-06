using System;
using UnityEngine;

public class LookAtComponent : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        this.transform.LookAt(target);
        transform.Rotate(0,180,0);
    }
}
