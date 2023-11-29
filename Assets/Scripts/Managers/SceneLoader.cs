using System.Collections;
using CustomClasses;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : InstanceClass<SceneLoader>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private string _sceneNameToBeLoaded;                     
    private bool _isSceneSync = false;                    // Booleano que controla si el cambio se realizara de manera Sincrona o Asincrona

    public void InstantLoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void LoadScene(string sceneName, bool isSceneSync)
    {
        _sceneNameToBeLoaded = sceneName;
        _isSceneSync = isSceneSync;

        StartCoroutine(InitializeSceneLoading());
    }

    public void LoadScene(string sceneName)
    {
        _sceneNameToBeLoaded = sceneName;
        _isSceneSync = true;

        StartCoroutine(InitializeSceneLoading());
    }

    private IEnumerator InitializeSceneLoading()
    {
        //Carga la escena de carga
        yield return SceneManager.LoadSceneAsync("LoadScene");

        StartCoroutine(ShowOverlayAndLoad());
    }

    private IEnumerator ShowOverlayAndLoad()
    {
        // Esperando unos segundos para evitar que cargue otra nueva escena 
        yield return new WaitForSeconds(1f);

        if (_isSceneSync)
        {
            // Si la escena debe cargarse como una escena multijugador, use PhotonNetwork.LoadLevel
            PhotonNetwork.LoadLevel(_sceneNameToBeLoaded);
        }
        else
        {
            // Si se trata de una carga de escena local, cargue la escena localmente. 
            var asyncLoad = SceneManager.LoadSceneAsync(_sceneNameToBeLoaded);

            while (!asyncLoad.isDone) yield return null;
            yield return null;
        }
    }
}