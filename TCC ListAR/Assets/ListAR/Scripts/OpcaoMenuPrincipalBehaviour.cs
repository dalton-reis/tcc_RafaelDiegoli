using UnityEngine;
using UnityEngine.SceneManagement;

public class OpcaoMenuPrincipalBehaviour : MonoBehaviour
{
    public string sceneToLoad;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnLoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
