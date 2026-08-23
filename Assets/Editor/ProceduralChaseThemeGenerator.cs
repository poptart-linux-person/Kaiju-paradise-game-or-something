#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace KaijuGame.EditorTools
{
    public static class ProceduralChaseThemeGenerator
    {
        private const int SampleRate = 44100;
        private const int Seconds = 16;
        private const float TwoPi = Mathf.PI * 2f;

        [MenuItem("Kaiju Game/Generate Procedural Chase Theme")]
        public static void Generate()
        {
            const string folder = "Assets/Audio/ProceduralChase";
            EnsureFolder(folder);

            CreateClip(folder + "/Chase_Base_Drone.asset", GenerateDrone());
            CreateClip(folder + "/Chase_Pulse.asset", GeneratePulse());
            CreateClip(folder + "/Chase_Percussion.asset", GeneratePercussion());
            CreateClip(folder + "/Chase_Panic_Lead.asset", GenerateLead());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Generated four synchronized procedural chase-theme layers in Assets/Audio/ProceduralChase.");
        }

        private static AudioClip GenerateDrone()
        {
            var samples = MakeBuffer();
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)SampleRate;
                float swell = 0.55f + 0.45f * Mathf.Sin(TwoPi * t / Seconds);
                float low = Mathf.Sin(TwoPi * 55f * t);
                float fifth = Mathf.Sin(TwoPi * 82.5f * t + 0.4f);
                samples[i] = 0.22f * swell * (0.72f * low + 0.28f * fifth) * Envelope(t);
            }
            return MakeClip("Chase_Base_Drone", samples);
        }

        private static AudioClip GeneratePulse()
        {
            var samples = MakeBuffer();
            const float bpm = 128f;
            float beat = 60f / bpm;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)SampleRate;
                float phase = Mathf.Repeat(t, beat) / beat;
                float pulse = Mathf.Exp(-phase * 14f);
                float sub = Mathf.Sin(TwoPi * 48f * t);
                samples[i] = 0.32f * pulse * sub * Envelope(t);
            }
            return MakeClip("Chase_Pulse", samples);
        }

        private static AudioClip GeneratePercussion()
        {
            var samples = MakeBuffer();
            const float bpm = 128f;
            float beat = 60f / bpm;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)SampleRate;
                float local = Mathf.Repeat(t, beat * 0.5f);
                float hit = Mathf.Exp(-local * 24f);
                float noise = HashNoise(i) * 0.65f + Mathf.Sin(TwoPi * 110f * t) * 0.35f;
                samples[i] = 0.18f * hit * noise * Envelope(t);
            }
            return MakeClip("Chase_Percussion", samples);
        }

        private static AudioClip GenerateLead()
        {
            var samples = MakeBuffer();
            const float bpm = 128f;
            float beat = 60f / bpm;
            int[] notes = { 0, 0, 3, 5, 3, 0, 7, 5 };
            float[] scale = { 110f, 116.54f, 130.81f, 146.83f, 155.56f, 174.61f, 196f, 207.65f };
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)SampleRate;
                int step = Mathf.FloorToInt(t / (beat * 0.5f)) % notes.Length;
                float inStep = Mathf.Repeat(t, beat * 0.5f) / (beat * 0.5f);
                float freq = scale[Mathf.Clamp(notes[step], 0, scale.Length - 1)];
                float gate = Mathf.Exp(-inStep * 7f);
                float saw = 2f * Mathf.Repeat(freq * t, 1f) - 1f;
                float edge = Mathf.Sin(TwoPi * freq * 2f * t);
                samples[i] = 0.095f * gate * (0.7f * saw + 0.3f * edge) * Envelope(t);
            }
            return MakeClip("Chase_Panic_Lead", samples);
        }

        private static float[] MakeBuffer() => new float[SampleRate * Seconds];

        private static AudioClip MakeClip(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static void CreateClip(string path, AudioClip clip)
        {
            if (AssetDatabase.LoadAssetAtPath<AudioClip>(path) != null)
                AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(clip, path);
        }

        private static float Envelope(float t)
        {
            float fade = Mathf.Clamp01(Mathf.Min(t, Seconds - t) * 2f);
            return fade * 0.98f;
        }

        private static float HashNoise(int x)
        {
            uint n = (uint)x;
            n ^= n << 13;
            n ^= n >> 17;
            n ^= n << 5;
            return ((n / (float)uint.MaxValue) * 2f) - 1f;
        }

        private static void EnsureFolder(string folder)
        {
            string[] parts = folder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
