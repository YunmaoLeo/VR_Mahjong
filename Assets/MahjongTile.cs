using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

public class MahjongTile : MonoBehaviour
{
    public TilesGenerator.TileInfo TileInfo;
    public Sprite tileSprite;
    [SerializeField] private MeshRenderer tileFaceRenderer;
    [SerializeField] private GameObject handGrabObject;
    [SerializeField] private Grabbable grabbable;
    public int Point = 5;

    private bool hasInitialized = false;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("SingleSnapSlot"))
        {
            // if is in grab status:
            if (grabbable.GrabPoints.Count > 0)
            {
                
            }
            // if it is not
            else
            {
                
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
