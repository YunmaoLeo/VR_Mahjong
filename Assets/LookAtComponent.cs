using System;
using UnityEngine;

public class LookAtComponent : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        this.transform.LookAt(target);
    }
}
