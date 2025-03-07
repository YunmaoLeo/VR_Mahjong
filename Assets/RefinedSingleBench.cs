using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RefinedSingleBench : MonoBehaviour
{
    [SerializeField] private List<RefinedSnapSlot> slots;

    private List<RefinedMahjongTile> _tilesInHold;

    [SerializeField] private bool debug_needDraw = false;
    [SerializeField] private bool debug_needThrow = false;

    [SerializeField] private Transform ThrowArea;
    [SerializeField] private Transform FunctionArea;
    
    [SerializeField] private TextMeshProUGUI infoText;
    
    [SerializeField] private Transform ComboPosition;
    [SerializeField] private float SingleComboWidth = 0.7f;
    [SerializeField] private float SingleComboHeight = 0.33f;
    [SerializeField] private float SingleTileWidth = 0.23f;
    [SerializeField] private int OneRowComboLength = 4;
    
    [SerializeField] private bool debug_needMoveHighlight = false;
    
    // draw tile, throw tile, 
    void Start()
    {
        DisEnableThrowArea();
        _tilesInHold = new List<RefinedMahjongTile>();

    }

    public void StartTurn()
    {
        //call network controller: xxx draw
        // detect Hu
        bool canHu = CanHu(GetTileInfos(), null);
        if (canHu)
        {
            //call network controller: xxx hu;
            Debug.Log("CanHu");
            return;
        }
        EnableThrowArea();
        //enable throw
    }

    public void EndTurn()
    {
        NetworkController.Instance.Broadcast_EndTurn();
        // call network controller: throw xxx
        // call network controller: end turn
    }

    public RefinedMahjongTile DrawTile()
    {
        //get first one
        var firstTile = TilesGenerator.Instance.GetFirstMahjong();
        //move to first available slot
        var targetSlot = GetFirstAvailableSlot();
        
        targetSlot.AssignNewTile(firstTile);

        return firstTile;
    }
    
    public void ShowTextWithDuration(float duration, string text)
    {
        infoText.transform.localScale = Vector3.zero;
        infoText.gameObject.SetActive(true);
        infoText.transform.DOScale(Vector3.one, 0.25f);
        infoText.text = text;
        StartCoroutine(EndTextWithDuration(duration));
    }

    private IEnumerator EndTextWithDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        infoText.gameObject.SetActive(false);
    }

    public void ThrowTile(RefinedSnapSlot slot)
    {
        if (slot.IsEmpty()) return;

        var tile = slot.CurrentTile;
        tile.DisablePhysicsComponents();
        slot.RemoveCurrentTile();
        
        TileThrowArea.Instance.ThrowTile(tile);
    }

    private RefinedSnapSlot GetFirstAvailableSlot()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                return slot;
            }
        }

        return slots.Last();
    }

    public List<TilesGenerator.TileInfo> GetTileInfos()
    {        
        List<TilesGenerator.TileInfo> tileInfos = new List<TilesGenerator.TileInfo>();
        
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                continue;
            }
            tileInfos.Add(slot.CurrentTile.TileInfo);
        }

        tileInfos.OrderBy(t => t.Type).ThenBy(t => t.Value).ToList();
        return tileInfos;
    }

    public void DetectConditions(RefinedMahjongTile newTile)
    {
        var newInfo = newTile.TileInfo;
        var tileInfos = GetTileInfos();

        if (CanHu(tileInfos, newInfo))
        {
            return;
        }

        if (CanGang(tileInfos, newInfo))
        {
            return;
        }
        
        if (CanPeng(tileInfos, newInfo))
        {
            return;
        }
    }

    IEnumerator EndTurnWithDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        FunctionArea.gameObject.SetActive(false);
        NetworkController.Instance.Broadcast_EndTurn();
    }

    private Vector3 initialScale = Vector3.one;
    private List<Tween> highlightTween = new List<Tween>();
    private List<RefinedSnapSlot> slotsInHightLight = new List<RefinedSnapSlot>();
    private void StopHighlight()
    {
        foreach (var tween in highlightTween)
        {
            tween.Kill();
        }
        
        highlightTween.Clear();
        foreach (var slot in slotsInHightLight)
        {
            slot.CurrentTile.transform.localScale = Vector3.one;
            slot.CurrentTile.IsAbleToFunction = false;
        }
        slotsInHightLight.Clear();
        FunctionArea.gameObject.SetActive(false);
    }
    private void ShowPengHighlight(TilesGenerator.TileInfo info)
    {
        int count = 0;
        for (var i = 0; i < slots.Count; i++)
        {
            if (count>= 2)
            {
                break;
            }

            var slot = slots[i];
            if (slot.IsEmpty()) continue;
            if (slot.CurrentTile.TileInfo.Equals(info))
            {
                slot.CurrentTile.IsAbleToFunction = true;
                count++;
                highlightTween.Add(slot.CurrentTile.transform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo));
                slotsInHightLight.Add(slot);
            }
        }
        
        Invoke(nameof(StopHighlight), 10f);
    }

    private void AddToTween(RefinedSnapSlot slot)
    {
        slot.CurrentTile.IsAbleToFunction = true;
        highlightTween.Add(slot.CurrentTile.transform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo));
        slotsInHightLight.Add(slot);
    }

    public RefinedSnapSlot FindFirstSlot(TilesGenerator.TileType type, int value)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty()) continue;
            if (slot.CurrentTile.TileInfo.Type == type && slot.CurrentTile.TileInfo.Value == value)
            {
                return slot;
            }
        }

        return null;
    }

    private void ShowChiHighlight(TilesGenerator.TileInfo newTile)
    {
        var hand = GetTileInfos();
        var values = hand.Where(t => t.Type == newTile.Type).Select(t => t.Value).Distinct().ToList();
        if (values.Contains(newTile.Value - 1) && values.Contains(newTile.Value + 1))
        {
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value - 1));
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value + 1));
        }
        else if (values.Contains(newTile.Value - 2) && values.Contains(newTile.Value - 1))
        {
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value - 2));
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value - 1));
        }
        else if (values.Contains(newTile.Value + 1) && values.Contains(newTile.Value + 2))
        {
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value + 2));
            AddToTween(FindFirstSlot(newTile.Type, newTile.Value + 1));
        }
        
        Invoke(nameof(StopHighlight), 10f);
    }

    public int currentComboNumber = 0;

    public void MoveHighlightTilesToFunctionArea()
    {
        List<RefinedMahjongTile> targetTiles = new List<RefinedMahjongTile>();
        if (slotsInHightLight.Count == 0)
        {
            return;
        }
        
        foreach (var slot in slotsInHightLight)
        {
            targetTiles.Add(slot.CurrentTile);
        }
        
        NetworkController.Instance.BroadcastCombo(targetTiles[0].TileInfo, targetTiles[1].TileInfo);

        targetTiles.Add(TileThrowArea.Instance.LastTile);

        int index = 0;
        foreach (var targetTile in targetTiles)
        {
            targetTile.parentSlot = null;
            targetTile.IsAbleToFunction = false;
            targetTile.DisablePhysicsComponents();
            
            //move to target position;
            var comboIndex = currentComboNumber;
            int row = (comboIndex / OneRowComboLength);
            int col = (comboIndex % OneRowComboLength);

            Vector3 position = ComboPosition.position + (transform.right * (-col * SingleComboWidth))
                                                      + (transform.forward * (row * SingleComboHeight));
                               
            position += transform.right * (SingleTileWidth * index);
            targetTile.transform.DOMove(position, 0.2f);
            var newRotation = transform.rotation * Quaternion.Euler(0, 0, 180);
            targetTile.transform.DORotate(newRotation.eulerAngles, 0.1f);
            
            index++;
        }
        
        StopHighlight();
        

        currentComboNumber++;
    }
    
    public bool CanPengOrCanChi(TilesGenerator.TileType type, int value)
    {
        FunctionArea.gameObject.SetActive(true);

        float duration = 5f;
        ShowTextWithDuration(duration, "Detecting Peng or Chi...");

        var newTileInfo = new TilesGenerator.TileInfo(type, value);
        var hand = GetTileInfos();
        var result = false;
        if (CanPeng(hand, newTileInfo))
        {
            duration = 10f;
            ShowTextWithDuration(duration, "Grab the tile to the front of the bench to Peng!");
            //show help context for peng
            //highlight first two newTileInfo
            ShowPengHighlight(newTileInfo);
            result = true;
        }
        else
        {
            if (CanChi(hand, newTileInfo))
            {
                duration = 10.0f;
                ShowTextWithDuration(duration, "Grab the tile to the front of the bench to Chi!");
                ShowChiHighlight(newTileInfo);
                result = true;
            }
        }

        StartCoroutine(EndTurnWithDuration(duration));
        
        return result;
    }

    public bool CanHu(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        if (hand.Count % 3 != 2) return false;
        return CheckHu(hand);
    }

    //back tracking
    private bool CheckHu(List<TilesGenerator.TileInfo> hand)
    {
        if (hand.Count == 0) return true;
        if (hand.Count == 2) return hand[0].Equals(hand[1]);
        
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Value == hand[i + 1].Value && hand[i].Value == hand[i + 2].Value)
            {
                var newHand = new List<TilesGenerator.TileInfo>(hand);
                newHand.RemoveRange(i, 3);
                if (CheckHu(newHand)) return true;
            }
        }

        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (hand[i].Type == hand[i + 1].Type && hand[i].Type == hand[i + 2].Type &&
                hand[i].Value + 1 == hand[i + 1].Value && hand[i].Value + 2 == hand[i + 2].Value)
            {
                var newHand = new List<TilesGenerator.TileInfo>(hand);
                newHand.RemoveAt(i + 2);
                newHand.RemoveAt(i + 1);
                newHand.RemoveAt(i);
                if (CheckHu(newHand)) return true;
            }
        }

        return false;
    }

    public bool CanPeng(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        return hand.Count(t => t.Equals(newTile)) >= 2;
    }

    public bool CanGang(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        return hand.Count(t => t.Equals(newTile)) >= 3;
    }

    public bool CanChi(List<TilesGenerator.TileInfo> hand, TilesGenerator.TileInfo newTile)
    {
        if (!hand.Any(t => t.Type == newTile.Type)) return false;

        var values = hand.Where(t => t.Type == newTile.Type).Select(t => t.Value).Distinct().ToList();
       

        return values.Contains(newTile.Value - 1) && values.Contains(newTile.Value + 1) ||
               values.Contains(newTile.Value - 2) && values.Contains(newTile.Value - 1) ||
               values.Contains(newTile.Value + 1) && values.Contains(newTile.Value + 2);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (debug_needDraw)
        {
            DrawTile();
            debug_needDraw = false;
        }

        if (debug_needThrow)
        {
            debug_needThrow = false;
            RefinedSnapSlot first = null;
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty()) first = slot;
            }
    
            first.ThrowTile(false);
        }

        if (debug_needMoveHighlight)
        {
            debug_needMoveHighlight = false;
            MoveHighlightTilesToFunctionArea();
        }
    }

    public void ResetTilePosition(RefinedSnapSlot oldSlot, RefinedMahjongTile tile)
    {
        float minDistance =float.MaxValue;
        RefinedSnapSlot targetSlot = null;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty()) continue;
            var distance = Vector3.Distance(slot.transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetSlot = slot;
            }
        }
        
        //move from tile position to target slot;
        targetSlot.AssignNewTile(tile);
        oldSlot.RemoveCurrentTile();   
    }

    public void EnableThrowArea()
    {
        ThrowArea.gameObject.SetActive(true);
    }
    
    public void DisEnableThrowArea()
    {
        ThrowArea.gameObject.SetActive(false);
    }

    public void OnThrowTileEvent(TilesGenerator.TileInfo tileInfo)
    {
        NetworkController.Instance.BroadcastThrowTile(tileInfo);
        DisEnableThrowArea();
        // EndTurn();
    }

    public void ThrowTileAccordingTypeInfo(TilesGenerator.TileType type, int value)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty()) continue;
            var info = slot.CurrentTile.TileInfo;
            if (info.Type == type && info.Value == value)
            {
                slot.ThrowTile(true);
            }
        }
    }

    public void DoLocalCombo(TilesGenerator.TileInfo info1, TilesGenerator.TileInfo info2)
    {
        //find first and second info2
        var firstSlot = FindFirstSlot(info1.Type, info1.Value);
        var firstTile = firstSlot.CurrentTile;
        firstSlot.RemoveCurrentTile();
        
        var secondSlot = FindFirstSlot(info2.Type, info2.Value);
        var secondTile = secondSlot.CurrentTile;
        secondSlot.RemoveCurrentTile();
        
        List<RefinedMahjongTile> tiles = new List<RefinedMahjongTile>();
        tiles.Add(firstTile);
        tiles.Add(secondTile);
        
        tiles.Add(TileThrowArea.Instance.LastTile);

        int index = 0;
        foreach (var targetTile in tiles)
        {
            targetTile.parentSlot = null;
            targetTile.IsAbleToFunction = false;
            targetTile.DisablePhysicsComponents();
            
            //move to target position;
            var comboIndex = currentComboNumber;
            int row = (comboIndex / OneRowComboLength);
            int col = (comboIndex % OneRowComboLength);

            Vector3 position = ComboPosition.position + (transform.right * (-col * SingleComboWidth))
                                                      + (transform.forward * (row * SingleComboHeight));
                               
            position += transform.right * (SingleTileWidth * index);
            targetTile.transform.DOMove(position, 0.2f);
            var newRotation = transform.rotation * Quaternion.Euler(0, 0, 180);
            targetTile.transform.DORotate(newRotation.eulerAngles, 0.1f);
            
            index++;
        }
        
    }
}
