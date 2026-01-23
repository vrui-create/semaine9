using UnityEngine;

public class ButtonLoaderScene : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;

    public void Load(string name)
    {
        SceneLoader.Instance.LoadScene(name, OnSceneLoaded);
    }

    void OnSceneLoaded()
    {
        Debug.Log("La scène est chargée et prête !");
    }

}
