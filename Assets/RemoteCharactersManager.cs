using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class RemoteCharactersManager : MonoBehaviour
{
    
    public List<CharacterCustomizer> remoteCharacters;
    
    private int currentCharacterIndex = 0 ;
    
    public Dictionary<int, CharacterCustomizer> characterDict = new Dictionary<int, CharacterCustomizer>();
    public Dictionary<int, PlayerAvatar> avatarDict = new Dictionary<int, PlayerAvatar>();
    
    public static RemoteCharactersManager Instance;
    
    void Start()
    {
        Instance = this;
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoinedInRemoteCharactersManager: " + player.PlayerId);
        if (runner.LocalPlayer.PlayerId == player.PlayerId)
        {
            return;
        }
        //activate one extra character and save in the dict.
        var character = remoteCharacters[currentCharacterIndex];
        currentCharacterIndex++;
        
        characterDict.Add(player.PlayerId, character);
        avatarDict.Add(player.PlayerId, character.GetComponent<PlayerAvatar>());
        
        //activate player 
        var initialScale = character.transform.localScale;
        character.gameObject.SetActive(true);
        character.transform.localScale = Vector3.zero;
        character.transform.DOScale(initialScale, 0.5f);
    }

    public void UpdateRemoteCharacter(int playerId, bool hasHat, bool hasBag, Vector3 hatColor, Vector3 bagColor)
    {
        if (characterDict[playerId] == null)
        {
            Debug.Log("No Remote Character Found");
            return;
        }
        var character = characterDict[playerId];
        character.UpdateAccessoryStatus(hasHat, hasBag, hatColor, bagColor);
        
    }

    public PlayerAvatar GetPlayerAvatar(int playerId)
    {
        return avatarDict[playerId];
    }


    public CharacterCustomizer GetCharacterCustomizer(int playerId)
    {
        return characterDict[playerId];
    }
    void Update()
    {
        
    }
}
