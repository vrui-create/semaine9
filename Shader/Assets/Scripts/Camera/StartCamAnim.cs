using UnityEngine;

public class StartCamAnim : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Animator animator;

    private string nextScene;
    public void StartAnimation(string scene)
    {
        canvas.enabled = false;
        nextScene = scene;
        animator.SetTrigger("StartAnim");
    }

    public void ChangeScene()
    {
        SceneLoader.Instance.LoadScene(nextScene);
    }
}
