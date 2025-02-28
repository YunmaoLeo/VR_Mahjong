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

    private NetworkRunner currentRunner;

    private bool isConnected = false;
    
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
        Debug.Log($"OnPlayerJoined with ID: {player.PlayerId}");
        //set current camera location
        var spawnPoint = possiblePositionsList[player.PlayerId - 1];
        if (player == runner.LocalPlayer)
        {
            CameraRigTransform.SetParent(spawnPoint.transform);
            CameraRigTransform.localPosition = Vector3.zero;
            CameraRigTransform.localRotation = Quaternion.identity;
        }
        // initialize other player's character
        // else
        {
            var characterTransform = possibleCharacterList[player.PlayerId - 1];
            var character = Instantiate(characterTransform);
            character.SetParent(spawnPoint.transform);
            character.localPosition = Vector3.zero;
            character.localRotation = Quaternion.identity;
            
            PlayerAvatarsDict.Add(player.PlayerId, character.GetComponent<PlayerAvatar>());
        }
        isConnected = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnected)
        {
            return;
        }
        
        

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

    [Rpc(sources:RpcSources.All, targets:RpcTargets.All)]
    public void RPC_UpdateHMDInfo(int playerId, Vector3 lookAtPosition, Quaternion rotation, Vector3 cameraLocation)
    {
        // if (currentRunner.LocalPlayer.PlayerId != playerId)
        {
            var character = PlayerAvatarsDict[playerId];
            character.HeadLookAtPos.position = lookAtPosition;
            character.transform.position =new Vector3( cameraLocation.x,character.transform.position.y,cameraLocation.z);

            
        }
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
