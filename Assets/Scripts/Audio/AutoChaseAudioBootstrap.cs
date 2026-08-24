using UnityEngine;
using System.Linq;
using KaijuGame.Modes;

namespace KaijuGame.Audio
{
    [DisallowMultipleComponent]
    public sealed class AutoChaseAudioBootstrap : MonoBehaviour
    {
        [SerializeField] private AudioClip ambient;
        [SerializeField] private AudioClip chase;
        [SerializeField] private AudioClip danger;
        [SerializeField] private AudioClip panic;
        [SerializeField] private ChaseAudioDirector director;

        private AudioSource ambientSource;
        private AudioSource chaseSource;
        private AudioSource dangerSource;
        private AudioSource panicSource;

        private void Awake()
        {
            director = director != null ? director : GetComponent<ChaseAudioDirector>();
            if (director == null) director = gameObject.AddComponent<ChaseAudioDirector>();
            ConfigureSources();
            BindThreats();
        }

        private void Update()
        {
            BindThreats();
            if (director != null)
            {
                var player = FindLocalPlayer();
                if (player != null) director.SetListenerTarget(player);
            }
        }

        private void ConfigureSources()
        {
            ambientSource = CreateSource("Ambient", ambient);
            chaseSource = CreateSource("Chase", chase);
            dangerSource = CreateSource("Danger", danger);
            panicSource = CreateSource("Panic", panic);
            director.ConfigureLayers(ambientSource, chaseSource, dangerSource, panicSource);
        }

        private AudioSource CreateSource(string name, AudioClip clip)
        {
            var child = transform.Find(name);
            var go = child != null ? child.gameObject : new GameObject(name);
            if (child == null) go.transform.SetParent(transform, false);

            var source = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            source.clip = clip;
            source.volume = 0f;
            if (clip != null && !source.isPlaying) source.Play();
            return source;
        }

        private void BindThreats()
        {
            var threats = FindObjectsOfType<ExtractionHunterAI>(true).Select(x => x.transform).ToArray();
            director?.SetThreats(threats);
        }

        private Transform FindLocalPlayer()
        {
            return GameObject.FindGameObjectsWithTag("Player")
                .Select(x => x.transform)
                .FirstOrDefault(x => x != null && x.gameObject.activeInHierarchy);
        }
    }
}
