using UnityEngine;

public class DDOL : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("Si activé, détruit automatiquement les doublons et garde seulement le premier instance.")]
    [SerializeField] private bool useSingleton = true;

    private static DDOL _instance;

    private void Awake()
    {
        if (useSingleton)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
