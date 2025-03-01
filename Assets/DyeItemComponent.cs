using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Oculus.Interaction;
using UnityEngine;

public class DyeItemComponent : MonoBehaviour
{
    [SerializeField] private Material dyeMaterial;
    [SerializeField] private Grabbable grabbable;

    private Transform _initialParentTransform;
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private Quaternion _initialRotation;

    public bool _isAttachable = false;
    public bool _isGrabbing = false;
    
    private List<GameObject> needToDyeList = new List<GameObject>();
    private HashSet<GameObject> needToDye = new HashSet<GameObject>();
    
    [TagField]
    [SerializeField] private string targetTag;

    
    
    void Start()
    {
        //store initial transforms
        _initialParentTransform = transform.parent;
        _initialPosition = transform.position;
        _initialScale = transform.localScale;
        _initialRotation = transform.rotation;
        
        //affect current item's material
        this.GetComponentInChildren<MeshRenderer>().material = dyeMaterial;
    }

    private void DyeColor(MeshRenderer otherRenderer)
    {
        var otherMat = otherRenderer.material;
        if (otherMat.name.StartsWith(dyeMaterial.name) || dyeMaterial.name.StartsWith(otherMat.name))
        {
            Debug.Log("Same Materials, do not dye");
            return;
        }

        var oriColor =otherMat.color;
        var newColor = dyeMaterial.color;
        
        otherRenderer.material = dyeMaterial;
        otherRenderer.material.color = oriColor;
        otherRenderer.material.DOColor(newColor, 0.5f);
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        return;
        if (!_isAttachable) return;
        if (other.CompareTag(targetTag))
        {
            //affect other's material
            DyeColor(other.GetComponent<MeshRenderer>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            needToDye.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (needToDye.Contains(other.gameObject))
            {
                needToDye.Remove(other.gameObject);
            }
        }
    }

    public void ReturnToOriginPlace()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.6f)
            .SetEase(Ease.OutBack);


    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        // update attachable status
        if (grabbable.GrabPoints.Count == 0)
        {
            if (_isGrabbing)
            {
                ReturnToOriginPlace();
                //check avaliable dye
                if (needToDye.Count > 0)
                {
                    foreach (var o in needToDye)
                    {
                        DyeColor(o.GetComponent<MeshRenderer>());
                    }
                }
            }
            _isGrabbing = false;
            _isAttachable = true;
        }
        else
        {
            _isGrabbing = true;
            _isAttachable = false;
        }
    }
}
