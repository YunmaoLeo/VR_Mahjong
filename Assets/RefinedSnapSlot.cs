using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class RefinedSnapSlot : MonoBehaviour
{
    [SerializeField] private RefinedSingleBench parentBench;

    public RefinedMahjongTile CurrentTile;

    [SerializeField] private float drawTileSpeed = 0.3f;
    
    public bool IsEmpty()
    {
        return CurrentTile == null;
    }

    public void AssignNewTile(RefinedMahjongTile tile)
    {
        CurrentTile = tile;
        tile.parentSlot = this;
        tile.IsInBench = true;
        tile.DisablePhysicsComponents();
        
        //move position;
        tile.transform.DOMove(this.transform.position, drawTileSpeed).OnComplete(() =>
        {
            tile.EnablePhysicsComponents();
        });
        tile.transform.DORotate(this.transform.rotation.eulerAngles, drawTileSpeed / 3);
    }

    public void ResetCurrentTilePosition()
    {
        parentBench.ResetTilePosition(this, CurrentTile);
    }

    public void RemoveCurrentTile()
    {
        CurrentTile = null;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ThrowTile()
    {
        CurrentTile.parentSlot = null;
        CurrentTile.IsInBench = false;
        CurrentTile.DisablePhysicsComponents();
        TileThrowArea.Instance.ThrowTile(CurrentTile);
        CurrentTile = null;
        
    }
}
