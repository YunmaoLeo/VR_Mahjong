using UnityEngine;
using UnityEngine.Serialization;

public class MahjongTile : MonoBehaviour
{
    public TilesGenerator.TileInfo TileInfo;
    public Sprite tileSprite;
    [SerializeField] private MeshRenderer tileFaceRenderer;
    [SerializeField] private GameObject handGrabObject;
    public int Point = 5;

    public void OnTileSnapped()
    {
        // show texture
        if (tileSprite != null)
        {
            tileFaceRenderer.material.mainTexture = tileSprite.texture;
        }
        
        // disable grabbable component
        handGrabObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
