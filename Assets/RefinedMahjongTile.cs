using System;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RefinedMahjongTile : MonoBehaviour
{
    
    public TilesGenerator.TileInfo TileInfo;
    [SerializeField] private bool isDebugMode;

    public RefinedSnapSlot parentSlot;
    
    public TilesGenerator.TileType Type { get; set; }
    public int Point { get; set; }
    
    [SerializeField] private GameObject handGrabObject;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private Rigidbody rb;

    private bool hasInitialized = false;

    [FormerlySerializedAs("isInBench")] public bool IsInBench = false;

    private bool isInCombo = false;

    [SerializeField] private bool debug_isGrab = false;

    [SerializeField] private Transform MahjongModelParent;

    [SerializeField] private BoxCollider boxCollider;

    private bool _isInGrab = false;
    private int _isInThrowArea = 0;
    
    public void InitializeMahjongModel()
    {
        var tileModel = MahjongPrefabManager.Instance.GetAccordingMahjongModel(Type, Point);
        Instantiate(tileModel, MahjongModelParent);
    }

    public void DisablePhysicsComponents()
    {
        this.rb.isKinematic = true;
        boxCollider.enabled = false;
        handGrabObject.SetActive(false);
    }

    public void EnablePhysicsComponents()
    {
        this.rb.isKinematic = false;
        boxCollider.enabled = true;
        handGrabObject.SetActive(true);
    }
    


    public void OnComboState()
    {
  
    }

    public void DestroyTile()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrowArea"))
        {
            Debug.Log("is in throw area");
            _isInThrowArea++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ThrowArea"))
        {
            _isInThrowArea--;
        }
    }

    void Start()
    {
        grabbable.WhenPointerEventRaised += OnPointerEvent;

    }
    void OnPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                _isInGrab = true;
                break;
            
            case PointerEventType.Unselect:
                _isInGrab = false;
                if (IsInBench)
                {
                    if (_isInThrowArea > 0)
                    {
                        parentSlot.ThrowTile();
                    }
                    else
                    {
                        parentSlot.ResetCurrentTilePosition();
                    }
                }
                
                //reset to nearest space
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}