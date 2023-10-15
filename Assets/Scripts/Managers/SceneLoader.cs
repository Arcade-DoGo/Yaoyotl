using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;                    

    private void Awake()
    {
        // Destruye este objeto si ya existe uno en la escena y no es este
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Guarda la instancia a este si no existe ninguno en la escena
        instance = this;
        // Vuelve indestructible al objeto, lo que se pasa entre diferentes escenas
        DontDestroyOnLoad(gameObject);
    }

    private string _sceneNameToBeLoaded;                     
    private bool _isSceneSync = false;                    // Booleano que controla si el cambio se realizara de manera Sincrona o Asincrona

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

        //Carga la escena actual
        // StartCoroutine(LoadActualyScene());
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

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield return null;
        }
    }
}