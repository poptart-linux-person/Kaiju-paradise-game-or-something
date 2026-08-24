using UnityEngine;
using System;
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
        [SerializeField] private string generatedFolder = "Assets/Audio/ProceduralChase";

        private ChaseAudioDirector director;
        private AudioSource ambientSource;
        private AudioSource chaseSource;
        private AudioSource dangerSource;
        private AudioSource panicSource;

        private void Awake()
        {
            director = GetComponent<ChaseAudioDirector>() ?? gameObject.AddComponent<ChaseAudioDirector>();
            ResolveGeneratedClips();
            ConfigureSources();
            BindThreats();
        }

        private void Update() => BindThreats();

        private void ResolveGeneratedClips()
        {
#if UNITY_EDITOR
            if (ambient == null) ambient = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"{generatedFolder}/Chase_Ambient.wav");
            if (chase == null) chase = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"{generatedFolder}/Chase_Pulse.wav");
            if (danger == null) danger = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"{generatedFolder}/Chase_Percussion.wav");
            if (panic == null) panic = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"{generatedFolder}/Chase_Panic.wav");
#endif
        }

        private void ConfigureSources()
        {
            ambientSource = CreateSource("Ambient", ambient, true);
            chaseSource = CreateSource("Chase", chase, true);
            dangerSource = CreateSource("Danger", danger, true);
            panicSource = CreateSource("Panic", panic, true);
            SetDirectorFields();
        }

        private AudioSource CreateSource(string name, AudioClip clip, bool loop)
        {
            var child = transform.Find(name);
            var go = child != null ? child.gameObject : new GameObject(name);
            if (child == null) go.transform.SetParent(transform, false);
            var source = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.clip = clip;
            source.volume = 0f;
            if (clip != null && !source.isPlaying) source.Play();
            return source;
        }

        private void SetDirectorFields()
        {
            var so = new UnityEditor.SerializedObject(director);
            so.FindProperty("ambientLayer").objectReferenceValue = ambientSource;
            so.FindProperty("chaseLayer").objectReferenceValue = chaseSource;
            so.FindProperty("dangerLayer").objectReferenceValue = dangerSource;
            so.FindProperty("listenerTarget").objectReferenceValue = FindLocalPlayer();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private void BindThreats()
        {
            var threats = FindObjectsOfType<ExtractionHunterAI>(true).Select(x => x.transform).ToArray();
            director.SetThreats(threats);
        }

        private Transform FindLocalPlayer()
        {
            return GameObject.FindGameObjectsWithTag("Player")
                .Select(x => x.transform)
                .FirstOrDefault(x => x != null && x.gameObject.activeInHierarchy);
        }
    }
}
