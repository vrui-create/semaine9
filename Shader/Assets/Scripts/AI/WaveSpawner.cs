using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class WaveDefinition
    {
        [Tooltip("Prefab d'ennemi à faire apparaitre pour cette vague")] public GameObject enemyPrefab;
        [Tooltip("Nombre total d'ennemis à spawner pour cette vague")] public int enemyCount = 5;
        [Tooltip("Rayon de spawn autour du centre")] public float spawnRadius = 5f;
        [Tooltip("Délai avant le début de la vague")] public float startDelay = 0f;
        [Tooltip("Intervalle entre chaque spawn (0 = tous en même temps)")] public float spawnInterval = 0f;
    }

    public enum WaveSpawnerState
    {
        Idle,
        Spawning,
        InWave,
        Completed
    }

    public class WaveSpawner : MonoBehaviour
    {
        [Header("Configuration des vagues")]
        [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
        [Tooltip("Centre de spawn. Si null, utilise la position de ce GameObject")]
        [SerializeField] private Transform spawnCenter;

        [Header("Options")]
        [Tooltip("Peut-on relancer les vagues une fois terminées ?")]
        [SerializeField] private bool canRestart = false;

        private int _currentWaveIndex = -1;
        private int _aliveEnemiesInWave = 0;
        private WaveSpawnerState _state = WaveSpawnerState.Idle;
        private Coroutine _spawnCoroutine;

        public event Action<int> OnWaveStarted;        // index de vague
        public event Action<int> OnWaveCompleted;      // index de vague
        public event Action OnAllWavesCompleted;

        public WaveSpawnerState State => _state;
        public int CurrentWaveIndex => _currentWaveIndex;
        public int AliveEnemiesInWave => _aliveEnemiesInWave;

        private void Awake()
        {
            if (spawnCenter == null)
            {
                spawnCenter = transform;
            }
        }

        public void StartWaves()
        {
            if (waves == null || waves.Count == 0)
            {
                Debug.LogWarning($"[WaveSpawner] Aucune vague configurée sur {name}");
                return;
            }

            if (_state == WaveSpawnerState.Completed && !canRestart)
            {
                return;
            }

            StopCurrentCoroutine();

            _currentWaveIndex = 0;
            _state = WaveSpawnerState.Idle;
            StartWave(_currentWaveIndex);
        }

        public void StartWave(int index)
        {
            if (index < 0 || index >= waves.Count)
            {
                Debug.LogError($"[WaveSpawner] Index de vague invalide {index} sur {name}");
                return;
            }

            StopCurrentCoroutine();
            _spawnCoroutine = StartCoroutine(SpawnWaveRoutine(index));
        }

        private IEnumerator SpawnWaveRoutine(int index)
        {
            _state = WaveSpawnerState.Spawning;
            _aliveEnemiesInWave = 0;

            var wave = waves[index];
            if (wave.enemyPrefab == null || wave.enemyCount <= 0)
            {
                Debug.LogWarning($"[WaveSpawner] Vague {index} mal configurée sur {name}");
                yield break;
            }

            if (wave.startDelay > 0f)
            {
                yield return new WaitForSeconds(wave.startDelay);
            }

            OnWaveStarted?.Invoke(index);

            int spawned = 0;
            while (spawned < wave.enemyCount)
            {
                SpawnEnemy(wave);
                spawned++;

                if (wave.spawnInterval > 0f && spawned < wave.enemyCount)
                {
                    yield return new WaitForSeconds(wave.spawnInterval);
                }
                else
                {
                    // Au moins un frame
                    yield return null;
                }
            }

            _state = WaveSpawnerState.InWave;
            _spawnCoroutine = null;
        }

        private void SpawnEnemy(WaveDefinition wave)
        {
            Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * wave.spawnRadius;
            Vector3 spawnPos = new Vector3(center.x + randomCircle.x, center.y, center.z + randomCircle.y);

            GameObject enemyObj = Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

            var characterManager = enemyObj.GetComponent<CharacterManager>();
            if (characterManager == null)
            {
                Debug.LogWarning($"[WaveSpawner] Le prefab {wave.enemyPrefab.name} n'a pas de CharacterManager. Le suivi de la vague ne fonctionnera pas correctement.");
            }
            else
            {
                _aliveEnemiesInWave++;
                characterManager.OnDeath += () => OnEnemyDied(characterManager);
            }
        }

        private void OnEnemyDied(CharacterManager manager)
        {
            if (_aliveEnemiesInWave <= 0)
                return;

            _aliveEnemiesInWave--;

            if (_aliveEnemiesInWave <= 0)
            {
                HandleWaveCompleted();
            }
        }

        private void HandleWaveCompleted()
        {
            if (_currentWaveIndex < 0 || _currentWaveIndex >= waves.Count)
                return;

            int completedWaveIndex = _currentWaveIndex;
            OnWaveCompleted?.Invoke(completedWaveIndex);

            _currentWaveIndex++;
            if (_currentWaveIndex >= waves.Count)
            {
                _state = WaveSpawnerState.Completed;
                OnAllWavesCompleted?.Invoke();
            }
            else
            {
                StartWave(_currentWaveIndex);
            }
        }

        private void StopCurrentCoroutine()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spawnCenter == null)
                spawnCenter = transform;
        }
#endif
    }
}
