using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fusion;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkController : NetworkBehaviour
{
    public static NetworkController Instance;
    [SerializeField] private List<Transform> possiblePositionsList;
    [SerializeField] private Transform CameraRigTransform;
    [SerializeField] private Transform HMDTransform;
    
    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private Transform RightHandTransform;

    public List<Transform> LobbyRoomTransforms;
    public List<Transform> GameRoomTransforms;
    
    public Dictionary<int, PlayerAvatar> PlayerAvatarsDict = new Dictionary<int, PlayerAvatar>();

    
    [SerializeField] private OVRManager _ovrManager;

    public bool isInGameMode = false;

    private NetworkRunner currentRunner;

    private bool isConnected = false;

    public bool IsLocalPlayerReady = false;

    [SerializeField] private CharacterCustomizer localCharacter;
    [SerializeField] private MeshRenderer bagRenderer;
    [SerializeField] private MeshRenderer hatRenderer;
    
    [SerializeField] private float accessoryUpdateDuration = 0.2f;
    
    private float _accessoryUpdateCd = 0.0f;
    
    public Dictionary<int, bool> IsPlayerReadyDict = new Dictionary<int, bool>();
    public bool haveAllPersonReady = false;
    public float readyStatusDuration = 5.0f;
    private float _readyStatusTimer = 0.0f;

    private Dictionary<int, int> PlayerPointDict = new Dictionary<int, int>();
    public bool IsGameRunning = false;

    [SerializeField] private string pickTileHelpInfo = "Please randomly pick 8 tiles.";
    [SerializeField] private string dropTileHelpInfo = "Please drop and pick at most 4 tiles.";
    
    public NetworkRunner GetRunner()
    {
        return currentRunner;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        currentRunner = runner;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        IsPlayerReadyDict[player.PlayerId] = false;
        PlayerPointDict[player.PlayerId] = 0;
        isConnected = true;
    }

    public void AddLocalPlayerPoints(int value)
    {
        PlayerPointDict[currentRunner.LocalPlayer.PlayerId] += value;
    }

    // Update is called once per frame

    public bool needReady = false;

    public enum GameState
    {
        ReadyToStart,
        GenerateTiles,
        DrawTiles,
        StartTurn,
    }

    private float readyToStartTimer = 0.0f;
    private float readyToStartDuration = 2.0f;

    private float generateTilesTimer = 0.0f;
    private float generateTilesDuration = 5.0f;

    private int drawTilesTotalCount = 0;
    private float drawTilesTimer = 0;
    private float drawTilesDuration = 0.4f;
    [SerializeField] private int initialDrawCount = 13;
    private int initialDrawIndex = 1;
    private int currentDrawId = 1;

    private float pickTilesTimer = 0.0f;
    [SerializeField] private float pickTilesDuration = 60.0f;

    private float dropTilesTimer = 0.0f;
    [SerializeField] private float dropTilesDuration = 30.0f;

    private float calculateFinalScoreTimer = 0.0f;
    private float calculateFinalScoreDuration = 10.0f;
    
    [SerializeField] private GameState _currentGameState = GameState.ReadyToStart;

    public int currentTurnId = 1;
    private int prevTurnId = 0;

    [SerializeField] private Transform currentTurnUI;
    
    public void GameStateUpdate()
    {
        switch (_currentGameState)
        {
            case GameState.ReadyToStart:
                readyToStartTimer += Time.fixedDeltaTime;
                if (readyToStartTimer >= readyToStartDuration)
                {
                    _currentGameState = GameState.GenerateTiles;
                }
                break;
            case GameState.GenerateTiles:
                // do generate tiles
                if (generateTilesTimer <= 0)
                {
                    TilesGenerator.Instance.GenerateTilesAndShuffle(42);
                }
                generateTilesTimer += Time.fixedDeltaTime;
                
                if (generateTilesTimer >= generateTilesDuration)
                {
                    _currentGameState = GameState.DrawTiles;
                }
                break;
            case GameState.DrawTiles:
                drawTilesTimer += Time.fixedDeltaTime;
                if (drawTilesTimer > drawTilesDuration)
                {
                    drawTilesTimer = 0f;
                    var bench = RefinedBenchCollection.Instance.GetSpecificBench(currentDrawId);
                    bench.DrawTile();
                    
                    currentDrawId = (currentDrawId % currentRunner.ActivePlayers.Count()) + 1;
                    Debug.Log("current DrawId: " + currentDrawId);
                    initialDrawIndex++;
                }
                else
                {
                    return;
                }
                
                drawTilesTotalCount = currentRunner.ActivePlayers.Count() * initialDrawCount;
                


                if (initialDrawIndex >= drawTilesTotalCount)
                {
                    _currentGameState = GameState.StartTurn;
                }
                
                break;
            
            
            case GameState.StartTurn:
                if (prevTurnId != currentTurnId)
                {
                    //change current turn image position;
                    currentTurnUI.gameObject.SetActive(true);
                    currentTurnUI.transform.DOMove(possiblePositionsList[currentTurnId - 1].position 
                                                   + new Vector3(0,8.0f,0), 0.25f);
                    
                    //end change ui
                    
                    prevTurnId = currentTurnId;
                    
                    var bench = RefinedBenchCollection.Instance.GetSpecificBench(
                        currentTurnId);
                    bench.DrawTile();
                    //if is current user
                    if (currentTurnId == currentRunner.LocalPlayer.PlayerId)
                    {
                        bench.StartTurn();
                    }
                }

                break;
        }
    }

    public void BroadcastThrowTile(TilesGenerator.TileInfo tileInfo)
    {
        RPC_ThrowTile(currentRunner.LocalPlayer.PlayerId, tileInfo.Type, tileInfo.Value);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_ThrowTile(int playerId, TilesGenerator.TileType type, int value)
    {
        if (playerId != currentRunner.LocalPlayer.PlayerId)
        {
            var bench = RefinedBenchCollection.Instance.GetSpecificBench(
                playerId);
            bench.ThrowTileAccordingTypeInfo(type, value);
        }
    }
    
    public void Broadcast_EndTurn()
    {
        currentTurnId = currentTurnId % currentRunner.ActivePlayers.Count()+1;
        // RPC_EndTurn(currentRunner.LocalPlayer.PlayerId);
    }
    

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_EndTurn(int playerId)
    {
        Debug.Log("RPC_EndTurn");
        if (playerId != currentRunner.LocalPlayer.PlayerId)
        {
            currentTurnId = currentTurnId % currentRunner.ActivePlayers.Count()+1;
        }
    }





    void FixedUpdate()
    {
        if (!isConnected)
        {
            return;
        }


        if (IsGameRunning || isInGameMode)
        {
            GameStateUpdate();
        }

        //handles avatar update things in game running mode
        if (IsGameRunning)
        {
            return;
            // update current HMDInfo and hand Info
            var rotation = Quaternion.LookRotation(HMDTransform.forward);
            // var lookDirection = HMDTransform.position - HMDTransform.forward * 10f;
            var lookLocation = HMDTransform.position + HMDTransform.forward * 1000f;
            var cameraLocation = HMDTransform.position;
            RPC_UpdateHMDInfo(currentRunner.LocalPlayer.PlayerId,lookLocation, rotation, cameraLocation);

            var rightHandPos = RightHandTransform.position;
            var rightHandRot = RightHandTransform.localRotation;
            var leftHandPos = LeftHandTransform.position;
            var leftHandRot = LeftHandTransform.localRotation;

            RPC_UpdatePlayerHandInfo(currentRunner.LocalPlayer.PlayerId,
                rightHandPos, rightHandRot, leftHandPos, leftHandRot);
            
        }

        // handle things in lobby room.
        if (IsGameRunning)
        {
            return;
        }
        
        //debug only
        if (needReady)
        {
            ChangeReadyStatus();
            needReady = false;
        }
        
        if (haveAllPersonReady)
        {
            _readyStatusTimer += Time.fixedDeltaTime;
            if (_readyStatusTimer >= readyStatusDuration)
            {
                StartGameStatus();
            }
        }
        else
        {
            _readyStatusTimer = 0.0f;
        }
        
        _accessoryUpdateCd += Time.deltaTime;
        if (_accessoryUpdateCd >= accessoryUpdateDuration)
        {
            _accessoryUpdateCd = 0;
            //update currentAccessory status
            var hatColor = hatRenderer.material.color;
            Vector3 VhatColor = new Vector3(hatColor.r, hatColor.g, hatColor.b);
            var bagColor = bagRenderer.material.color;
            Vector3 VbagColor = new Vector3(bagColor.r, bagColor.g, bagColor.b);
            RPC_UpdateAccessoryInfo(currentRunner.LocalPlayer.PlayerId, localCharacter.GetHatOnStatus(), localCharacter.GetBagOnStatus(),
                VhatColor, VbagColor);
        }
        
  
    }

    public void ChangeReadyStatus()
    {
        IsLocalPlayerReady = !IsLocalPlayerReady;
        localCharacter.ChangeCheckStatus(IsLocalPlayerReady);
        RPC_UpdateReadyStatus(currentRunner.LocalPlayer.PlayerId, IsLocalPlayerReady);
    }


    public void StartGameStatus()
    {
        
        IsGameRunning = true;
        
        //hide and show room settings accordingly.
        foreach (var t in LobbyRoomTransforms)
        {
            t.gameObject.SetActive(false);
        }
        foreach (var t in GameRoomTransforms)
        {
            t.gameObject.SetActive(true);
        }

        SetLocalPlayerCharacter();
        AllocateRemoteCharacters();
    }

    public void SetLocalPlayerCharacter()
    {
        localCharacter.gameObject.SetActive(false);
        var playerId = currentRunner.LocalPlayer.PlayerId;
        // PlayerAvatarsDict.Add(playerId, RemoteCharactersManager.Instance.avatarDict[playerId]);
        
        //set local camera
        var localPlayerID = currentRunner.LocalPlayer.PlayerId;
        var spawnPoint = possiblePositionsList[localPlayerID - 1];
        CameraRigTransform.SetParent(spawnPoint.transform);
        CameraRigTransform.localPosition = Vector3.zero;
        CameraRigTransform.localRotation = Quaternion.identity;
    }

    public void AllocateRemoteCharacters()
    {
        foreach (var kv in RemoteCharactersManager.Instance.avatarDict)
        {
            var playerId = kv.Key;
            var character = kv.Value.transform;
            if (playerId != currentRunner.LocalPlayer.PlayerId)
            {
                var spawnPoint = possiblePositionsList[playerId - 1];
               
                character.SetParent(spawnPoint.transform);
                character.localPosition = Vector3.zero;
                character.localRotation = Quaternion.identity;
                character.localScale = Vector3.one * 8;
                
                PlayerAvatarsDict.Add(playerId, RemoteCharactersManager.Instance.avatarDict[playerId]);
                RemoteCharactersManager.Instance.characterDict[playerId].ChangeCheckStatus(false);
                
                character.gameObject.SetActive(true);
            }
        }
    }

    public void BroadcastCurrentPlayerScoreInfo(int score)
    {
        RPC_UpdatePlayerScoreInfo(currentRunner.LocalPlayer.PlayerId, score);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_UpdatePlayerScoreInfo(int playerId, int score)
    {
        if (playerId != currentRunner.LocalPlayer.PlayerId)
        {
            GameInfoTextManager.Instance.UpdatePlayerInfoText(playerId, score);
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_UpdateReadyStatus(int playerId, bool readyStatus)
    {
        // update player ready status
        IsPlayerReadyDict[playerId] = readyStatus;
        
        //update game appearance
        if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = RemoteCharactersManager.Instance.GetCharacterCustomizer(playerId);
            character.ChangeCheckStatus(readyStatus);
        }
        
        //update have all person ready status
        if (IsPlayerReadyDict.Count > 1)
        {
            haveAllPersonReady = true;
            
            foreach (var kv in IsPlayerReadyDict)
            {
                if (kv.Value == false)
                {
                    haveAllPersonReady = false;
                    _readyStatusTimer = 0.0f;
                    break;
                }
            }

        }
    }

    [Rpc(sources:RpcSources.All, targets:RpcTargets.All)]
    public void RPC_UpdateHMDInfo(int playerId, Vector3 lookAtPosition, Quaternion rotation, Vector3 cameraLocation)
    {
        if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = RemoteCharactersManager.Instance.GetPlayerAvatar(playerId);
            character.HeadLookAtPos.position = lookAtPosition;
            if (isInGameMode)
            {
                character.transform.position =new Vector3( cameraLocation.x,character.transform.position.y,cameraLocation.z);
            }
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_UpdateAccessoryInfo(int playerId, bool isHatOn, bool isBagOn, Vector3 hatColor, Vector3 bagColor)
    {
        if (currentRunner.LocalPlayer.PlayerId == playerId)
        {
            return;
        }
        
        RemoteCharactersManager.Instance.UpdateRemoteCharacter(playerId, isHatOn, isBagOn, hatColor, bagColor);
    }


    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_UpdatePlayerHandInfo(int playerId, Vector3 rightHandPos, Quaternion rightHandRotation,
        Vector3 leftHandPos, Quaternion leftHandRotation)
    {
        if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = RemoteCharactersManager.Instance.GetPlayerAvatar(playerId);
            Quaternion offsetRotation = Quaternion.Euler(180, 90, 0);
            character.RightHandTarget.position = rightHandPos;
            character.RightHandTarget.localRotation = rightHandRotation * offsetRotation;
            character.LeftHandTarget.position = leftHandPos;
            character.LeftHandTarget.localRotation = leftHandRotation * offsetRotation;
        }
    }
}
