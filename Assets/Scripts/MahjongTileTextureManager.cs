using System;
using System.Collections.Generic;
using UnityEngine;

public class MahjongTileTextureManager : MonoBehaviour
{
    public static MahjongTileTextureManager Instance;

    private Dictionary<string, Sprite> _spritesDict = new Dictionary<string, Sprite>();
    
    [SerializeField] private List<Sprite> spritesList;

    private void Awake()
    {
    }

    public Sprite GetSprite(string spriteName)
    {
        if (_spritesDict.ContainsKey(spriteName))
        {
            Debug.Log($"successfullyFind {spriteName}");
            return _spritesDict[spriteName];
        }
        else
        {
            Debug.Log($"fail to find {spriteName}");
            return null;
        }
    }

    void Start()
    {
        Instance = this;
        foreach (var sprite in spritesList)
        {
            _spritesDict.Add(sprite.name, sprite);
        }
    }

    void Update()
    {
        
    }
}
