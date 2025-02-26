using System.Collections;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSceneManager : MonoBehaviour
{
    
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private OVRPassthroughLayer passthroughLayer;

    [SerializeField] private float passthroughDropRate = 0.5f;
    
    [Space]
    [SerializeField] private string majongScene = "WhiteBoxScene";
    public static IntroSceneManager Instance { get; private set; }

    private bool isVideoTriggered = false;

    private AsyncOperation asyncSceneOperation;

    void Start()
    {
        Instance = this;
        
        videoPlayer.loopPointReached += StartGameScene;
    }

    private void StartGameScene(VideoPlayer source)
    {
        //load scene
        if (asyncSceneOperation != null)
        {
            asyncSceneOperation.allowSceneActivation = true;
        }
        
        
    }

    public void OnStartIntroVideo()
    {
        if (!isVideoTriggered)
        {
            passthroughLayer.textureOpacity = 1.0f - passthroughDropRate;
            isVideoTriggered = true;
            videoPlayer.Play();
        }
    }

    // Update is called once per frame

    private bool hasPreLoad = false;
    
    
    void PreLoadScene()
    {
        if (hasPreLoad)
        {
            return;
        }
        hasPreLoad = true;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        asyncSceneOperation = SceneManager.LoadSceneAsync(majongScene);
        asyncSceneOperation.allowSceneActivation = false;

        while (asyncSceneOperation.progress < 0.9f)
        {
            Debug.Log($"Loading Progress: {asyncSceneOperation.progress}");
            yield return null;
        }
        
        Debug.Log(asyncSceneOperation.progress);
        
    }
    
    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            float progress = (float)(videoPlayer.time / videoPlayer.length);
            passthroughLayer.textureOpacity = (1.0f - passthroughDropRate) * (1.0f - progress);

            if (progress >= 0.8f)
            {
                PreLoadScene();
            }
            
        }
    }
}
