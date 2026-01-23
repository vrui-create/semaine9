using UnityEngine;
using Game.Level;

namespace Game.Level
{
    [RequireComponent(typeof(Collider))]
    public class RoomTrigger : MonoBehaviour
    {
        [Tooltip("Contrôleur de salle qui gère les vagues et les portes")] 
        [SerializeField] private WaveRoomController roomController;
        [Tooltip("Tag du joueur qui déclenche les vagues")] 
        [SerializeField] private string playerTag = "Player";

        private bool _triggerUsed;

        private void Awake()
        {
            if (roomController == null)
            {
                roomController = GetComponentInParent<WaveRoomController>();
            }

            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggerUsed) return;
            if (!other.CompareTag(playerTag)) return;

            _triggerUsed = true;

            if (roomController != null)
            {
                roomController.TriggerWaves();
            }
            else
            {
                Debug.LogWarning($"[RoomTrigger] Aucun WaveRoomController trouvé pour {name}");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            if (roomController == null)
            {
                roomController = GetComponentInParent<WaveRoomController>();
            }
        }
#endif
    }
}

