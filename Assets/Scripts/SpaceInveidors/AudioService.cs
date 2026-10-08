using UnityEngine.Audio;

namespace SpaceInveidors.Sound
{
    using UnityEngine;

    [DefaultExecutionOrder(-90)]
    public class AudioService : MonoBehaviour, ISoundProvider
    {
        public static AudioService Instance { get; private set; }

        [Header("Clips")]
        [SerializeField] private AudioClip _playerShotClip;
        [SerializeField] private AudioClip _enemyShotClip;
        [SerializeField] private AudioClip _enemyDeathClip;

        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup _mixerGroup;

        [Header("Volumes")]
        [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float _playerShotVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float _enemyShotVolume = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _enemyDeathVolume = 0.8f;

        [Header("Pitch")]
        [SerializeField] private Vector2 _pitchRange = new Vector2(0.95f, 1.05f);

        [Header("Sources")]
        [Tooltip("Сколько одновременных звуков может играть.")]
        [SerializeField] private int _maxSources = 16;

        private AudioSource[] _sources;
        private int _sourceIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CreateSources();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        private void CreateSources()
        {
            _maxSources = Mathf.Max(1, _maxSources);
            _sources = new AudioSource[_maxSources];

            for (int i = 0; i < _sources.Length; i++)
            {
                GameObject sourceObject = new GameObject($"AudioSource_{i}");
                sourceObject.transform.SetParent(transform, false);

                AudioSource source = sourceObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;

                if (_mixerGroup != null)
                {
                    source.outputAudioMixerGroup = _mixerGroup;
                }

                _sources[i] = source;
            }
        }

        public void PlayPlayerShot()
        {
            PlayClip(_playerShotClip, _playerShotVolume);
        }

        public void PlayEnemyShot()
        {
            PlayClip(_enemyShotClip, _enemyShotVolume);
        }

        public void PlayEnemyDeath()
        {
            PlayClip(_enemyDeathClip, _enemyDeathVolume);
        }

        private void PlayClip(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            if (_sources == null || _sources.Length == 0)
            {
                CreateSources();
            }

            if (_sources == null || _sources.Length == 0)
            {
                return;
            }

            AudioSource source = _sources[_sourceIndex];
            _sourceIndex = (_sourceIndex + 1) % _sources.Length;

            source.volume = Mathf.Clamp01(volume * _masterVolume);
            source.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            source.PlayOneShot(clip);
        }
    }
}