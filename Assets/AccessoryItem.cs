using System;
using Cinemachine;
using Oculus.Interaction;
using UnityEngine;

public class AccessoryItem : MonoBehaviour
{
    [TagField]
    [SerializeField] private string targetSlotTag;

    public bool isAttachable = true;
    public bool isWearing = false;
    private Vector3 _initialLocalScale;
    private Transform _initialParentTransform;
    [SerializeField] private Grabbable _grabbable;
    
    //1. if in grab status (onTriggerEnter)
    //2. 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetSlotTag))
        {
            if (!isAttachable)
            {
                return;
            }
            if (isWearing)
            {
                return;
            }

            //disable grab;
            // _grabbable.gameObject.SetActive(false);
             
            transform.parent = other.transform;
            transform.position = other.transform.position;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            isWearing = true;
        }
    }

    void Start()
    {
        _initialLocalScale = transform.localScale;
        _initialParentTransform = transform.parent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if is grabbed;
        if (isWearing)
        {
            if (_grabbable.GrabPoints.Count > 0)
            {
                isWearing = false;
                transform.parent = _initialParentTransform;
                transform.localScale = _initialLocalScale;
            }
        }
        
        // update attachable status
        if (_grabbable.GrabPoints.Count == 0)
        {
            isAttachable = true;
        }
        else
        {
            isAttachable = false;
        }
    }
}
