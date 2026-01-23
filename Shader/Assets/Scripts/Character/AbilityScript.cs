using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    [SerializeField] private float SlowSpeed = 0.3f;
    private bool slow = false;
    public void SlowTime()
    {
        slow = !slow;
        if (slow)
        {
            Time.timeScale = 0.3f;

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    
}
