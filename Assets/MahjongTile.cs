using System;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

public class MahjongTile : NetworkBehaviour
{
    
    public TilesGenerator.TileInfo TileInfo;
    [SerializeField] private bool isDebugMode;

    [Networked] public TilesGenerator.TileType Type { get; set; }
    [Networked] public int Point { get; set; }
    
    [SerializeField] private MeshRenderer tileFaceRenderer;
    [SerializeField] private GameObject handGrabObject;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private List<Collider> collideSlots;
    
    [SerializeField] private Color tileColor;

    private bool hasInitialized = false;

    private bool isInSnap = false;

    private bool isInCombo = false;

    [SerializeField] private bool debug_isGrab = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SingleSnapSlot"))
        {
            collideSlots.Add(other);
        }
    }

    public void OnComboState()
    {
        handGrabObject.SetActive(false);
        rb.isKinematic = true;
        isInCombo = true;
    }

    public void DestroyTile()
    {
        if (isInSnap)
        {
            foreach (var collideSlot in collideSlots)
            {
                collideSlot.GetComponent<SingleSnapSlot>().RemoveTile(this);
            }
        }
        NetworkController.Instance.GetRunner().Despawn(this.GetComponent<NetworkObject>());
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInCombo)
        {
            return;
        }

        if (other.CompareTag("SingleSnapSlot"))
        {
            // if is in grab status:
            if (grabbable.GrabPoints.Count > 0 || debug_isGrab)
            {
                rb.isKinematic = false;
                // if is snapped, remove
                if (isInSnap)
                {
                    var slot = other.GetComponent<SingleSnapSlot>();
                    if (slot.RemoveTile(this))
                    {
                        isInSnap = false;
                    }
                }
                //not able to snap
            }
            // if it is not
            else
            {
                if (!isInSnap)
                {
                    // snap to slot
                    var slot = other.GetComponent<SingleSnapSlot>();

                    if (slot.SnapToSlot(this))
                    {
                        isInSnap = true;
                        rb.isKinematic = true;


                        //initialize it if it haven't 
                        if (!hasInitialized)
                        {
                            string spriteName;
                            if (isDebugMode)
                            {
                                spriteName = Type.ToString().ToLower();
                                spriteName += Point.ToString();
                                Debug.Log(spriteName);
                            }
                            else
                            {
                                spriteName = Type.ToString().ToLower() + Point.ToString();
                            }

                            var sprite = MahjongTileTextureManager.Instance.GetSprite(spriteName).texture;
                            tileFaceRenderer.material.SetTexture("_BaseMap", sprite);
                            tileFaceRenderer.material.SetColor("_BaseColor", tileColor);
                            tileFaceRenderer.material.SetColor("_EmissionColor", tileColor * 2.5f);

                            hasInitialized = true;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SingleSnapSlot"))
        {
            if (collideSlots.Contains(other))
            {
                collideSlots.Remove(other);
            }
        }
    }

    void Start()
    {
        if (isDebugMode)
        {
            TileInfo = new TilesGenerator.TileInfo(Type, Point);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (grabbable.GrabPoints.Count > 0 || debug_isGrab || collideSlots.Count == 0)
        {
            rb.isKinematic = false;
        }
    }
}