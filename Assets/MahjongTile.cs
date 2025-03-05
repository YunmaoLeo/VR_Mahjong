using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

public class MahjongTile : MonoBehaviour
{
    public TilesGenerator.TileInfo TileInfo;
    [SerializeField] private bool isDebugMode;
    [SerializeField] private TilesGenerator.TileType type;
    [SerializeField] private int point;
    public Sprite tileSprite;
    [SerializeField] private MeshRenderer tileFaceRenderer;
    [SerializeField] private GameObject handGrabObject;
    [SerializeField] private Grabbable grabbable;

    [SerializeField] private Color tileColor;
    public int Point = 5;

    private bool hasInitialized = false;

    private bool isInSnap = false;

    [SerializeField] private bool debug_isGrab = true;

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("SingleSnapSlot"))
        {
            // if is in grab status:
            if (grabbable.GrabPoints.Count > 0 || debug_isGrab)
            {
                // if is snapped, remove
                if (isInSnap)
                {
                    var slot = other.GetComponent<SingleSnapSlot>();
                    slot.RemoveTile(this);
                    isInSnap = false;
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
                    slot.SnapToSlot(this);
                    isInSnap = true;


                    //initialize it if it haven't 
                    if (!hasInitialized)
                    {
                        string spriteName;
                        if (isDebugMode)
                        {
                            spriteName = type.ToString().ToLower();
                            spriteName += point.ToString();
                            Debug.Log(spriteName);
                        }
                        else
                        {
                            spriteName = TileInfo.GetSpriteName();
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

    private void OnTriggerExit(Collider other)
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}