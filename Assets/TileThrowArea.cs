using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class TileThrowArea : MonoBehaviour
{
    public static TileThrowArea Instance;

    [SerializeField] private float tileWidth = 0.1f;
    [SerializeField] private float tileHeight = 0.2f;
    [SerializeField] private int lineLength = 6;
    private List<RefinedMahjongTile> throwedTileList;

    public RefinedMahjongTile LastTile;

    void Start()
    {
        throwedTileList = new List<RefinedMahjongTile>();
        Instance = this;
    }

    public void ThrowTile(RefinedMahjongTile tile)
    {
        var index = throwedTileList.Count;
        int row = (index / lineLength);
        int col = (index % lineLength);

        Vector3 position = transform.position + new Vector3(col * tileWidth, 0, -row * tileHeight);

        tile.transform.DOMove(position, 0.3f);
        tile.transform.DOLocalRotate(new Vector3(0,0,180), 0.1f);
        throwedTileList.Add(tile);
        LastTile = tile;
    }

    // Update is called once per frame
    void Update()
    {
    }
}