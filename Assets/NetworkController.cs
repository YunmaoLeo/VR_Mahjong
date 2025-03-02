using System.Collections.Generic;
using Fusion;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkController : NetworkBehaviour
{
    [SerializeField] private List<Transform> possiblePositionsList;

    [SerializeField] private List<Transform> possibleCharacterList;
    [SerializeField] private Transform CameraRigTransform;
    [SerializeField] private Transform HMDTransform;
    
    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private Transform RightHandTransform;
    
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        currentRunner = runner;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Debug.Log($"OnPlayerJoined with ID: {player.PlayerId}");
        // //set current camera location
        // var spawnPoint = possiblePositionsList[player.PlayerId - 1];
        // if (player == runner.LocalPlayer)
        // {
        //     CameraRigTransform.SetParent(spawnPoint.transform);
        //     CameraRigTransform.localPosition = Vector3.zero;
        //     CameraRigTransform.localRotation = Quaternion.identity;
        // }
        // // initialize other player's character
        // // else
        // {
        //     var characterTransform = possibleCharacterList[player.PlayerId - 1];
        //     var character = Instantiate(characterTransform);
        //     character.SetParent(spawnPoint.transform);
        //     character.localPosition = Vector3.zero;
        //     character.localRotation = Quaternion.identity;
        //     
        //     PlayerAvatarsDict.Add(player.PlayerId, character.GetComponent<PlayerAvatar>());
        // }
        isConnected = true;
    }

    // Update is called once per frame

    public bool needReady = false;
    void Update()
    {
        if (!isConnected)
        {
            return;
        }

        if (needReady)
        {
            ChangeReadyStatus();
            needReady = false;
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
        
        // update current HMDInfo and hand Info
        var rotation = Quaternion.LookRotation(HMDTransform.forward);
        // var lookDirection = HMDTransform.position - HMDTransform.forward * 10f;
        var lookLocation = HMDTransform.position + HMDTransform.forward * 1000f;
        var cameraLocation = HMDTransform.position;
        RPC_UpdateHMDInfo(currentRunner.LocalPlayer.PlayerId,lookLocation, rotation, cameraLocation);


        if (isInGameMode)
        {
            var rightHandPos = RightHandTransform.position;
            var rightHandRot = RightHandTransform.localRotation;
            var leftHandPos = LeftHandTransform.position;
            var leftHandRot = LeftHandTransform.localRotation;
        
            RPC_UpdatePlayerHandInfo(currentRunner.LocalPlayer.PlayerId,
                rightHandPos, rightHandRot, leftHandPos, leftHandRot);
        }
    }

    public void ChangeReadyStatus()
    {
        IsLocalPlayerReady = !IsLocalPlayerReady;
        localCharacter.ChangeCheckStatus(IsLocalPlayerReady);
        RPC_UpdateReadyStatus(currentRunner.LocalPlayer.PlayerId, IsLocalPlayerReady);
    }


    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_UpdateReadyStatus(int playerId, bool readyStatus)
    {
        if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = RemoteCharactersManager.Instance.GetCharacterCustomizer(playerId);
            character.ChangeCheckStatus(readyStatus);
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
        // if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = PlayerAvatarsDict[playerId];
            Quaternion offsetRotation = Quaternion.Euler(180, 90, 0);
            character.RightHandTarget.position = rightHandPos;
            character.RightHandTarget.localRotation = rightHandRotation * offsetRotation;
            character.LeftHandTarget.position = leftHandPos;
            character.LeftHandTarget.localRotation = leftHandRotation * offsetRotation;
        }
    }
}
