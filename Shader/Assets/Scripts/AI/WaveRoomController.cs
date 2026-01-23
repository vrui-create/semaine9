using System.Collections.Generic;
using UnityEngine;
using Game.AI;

namespace Game.Level
{
    public class WaveRoomController : MonoBehaviour
    {
        [Header("Références")] 
        [SerializeField] private WaveSpawner waveSpawner;
        [Tooltip("Colliders à activer pour fermer la salle pendant les vagues")] 
        [SerializeField] private List<Collider> roomBlockers = new List<Collider>();

        [Header("Options")] 
        [Tooltip("Fermer la salle dès le début de la première vague")] 
        [SerializeField] private bool lockOnFirstWave = true;

        private bool _isCompleted;

        private void Awake()
        {
            if (waveSpawner == null)
            {
                waveSpawner = GetComponentInChildren<WaveSpawner>();
            }
        }

        private void OnEnable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnWaveStarted += HandleWaveStarted;
                waveSpawner.OnAllWavesCompleted += HandleAllWavesCompleted;
            }

            // Salle ouverte par défaut
            SetRoomLocked(false);
        }

        private void OnDisable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnWaveStarted -= HandleWaveStarted;
                waveSpawner.OnAllWavesCompleted -= HandleAllWavesCompleted;
            }
        }

        public void TriggerWaves()
        {
            if (_isCompleted)
                return;

            if (waveSpawner == null)
            {
                Debug.LogWarning($"[WaveRoomController] Aucun WaveSpawner référencé sur {name}");
                return;
            }

            waveSpawner.StartWaves();
        }

        private void HandleWaveStarted(int waveIndex)
        {
            if (!lockOnFirstWave)
                return;

            if (waveIndex == 0)
            {
                SetRoomLocked(true);
            }
        }

        private void HandleAllWavesCompleted()
        {
            _isCompleted = true;
            SetRoomLocked(false);
        }

        private void SetRoomLocked(bool locked)
        {
            if (roomBlockers == null)
                return;

            foreach (var col in roomBlockers)
            {
                if (col == null) continue;
                col.enabled = locked;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (waveSpawner == null)
            {
                waveSpawner = GetComponentInChildren<WaveSpawner>();
            }
        }
#endif
    }
}

