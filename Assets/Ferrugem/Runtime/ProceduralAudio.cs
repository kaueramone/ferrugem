using System;
using UnityEngine;

namespace Ferrugem
{
    public enum SoundCue { Shot, Reload, Step, Impact, Zombie, Blast }

    // Original synthesized placeholders: no recordings, downloaded audio or generative service.
    // This component exists only in client presentation and does not drive gameplay.
    public sealed class ProceduralAudio : MonoBehaviour
    {
        private static ProceduralAudio instance;
        private readonly AudioClip[] clips = new AudioClip[6];
        private readonly AudioSource[] voices = new AudioSource[24];
        private readonly bool[] logged = new bool[6];
        private int nextVoice;
        private void Awake()
        {
            instance = this;
            for (int i = 0; i < clips.Length; ++i) clips[i] = Synthesize((SoundCue)i);
            for (int i = 0; i < voices.Length; ++i)
            {
                var voice = new GameObject("Procedural audio voice").AddComponent<AudioSource>();
                voice.transform.SetParent(transform, false);
                voice.playOnAwake = false; voice.rolloffMode = AudioRolloffMode.Linear;
                voice.minDistance = 1.5f; voice.maxDistance = 35; voice.dopplerLevel = 0;
                voices[i] = voice;
            }
            Debug.Log("[Ferrugem] AUDIO_READY clips=6 voices=24 synthesized=1");
        }
        public static void Play(SoundCue cue, Vector3 position, bool local = false)
        {
            if (instance == null || FerrugemBootstrap.Server) return;
            if (AudioListener.pause) return;
            var source = instance.voices[instance.nextVoice++ % instance.voices.Length];
            source.Stop(); source.transform.position = position;
            source.spatialBlend = local ? 0 : 1;
            source.volume = cue == SoundCue.Step ? 0.22f : cue == SoundCue.Shot ? 0.45f : 0.33f;
            source.pitch = 1; source.clip = instance.clips[(int)cue]; source.Play();
            if (!instance.logged[(int)cue])
            {
                instance.logged[(int)cue] = true;
                Debug.Log($"[Ferrugem] AUDIO_CUE cue={cue} synthesized=1");
            }
        }
        private static AudioClip Synthesize(SoundCue cue)
        {
            const int rate = 22050;
            float duration = cue == SoundCue.Zombie ? 0.7f : cue == SoundCue.Blast ? 0.6f : cue == SoundCue.Reload ? 0.38f : 0.24f;
            var data = new float[Mathf.CeilToInt(rate * duration)];
            var random = new System.Random(734 + (int)cue * 19);
            float filtered = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                float time = (float)i / rate;
                float noise = (float)(random.NextDouble() * 2 - 1);
                filtered += (noise - filtered) * 0.18f;
                float sample;
                switch (cue)
                {
                    case SoundCue.Shot:
                        sample = noise * Mathf.Exp(-time * 45) * 0.55f + Mathf.Sin(time * 2 * Mathf.PI * 95) * Mathf.Exp(-time * 25) * 0.4f; break;
                    case SoundCue.Reload:
                        var clicks = Mathf.Exp(-Mathf.Abs(time - 0.03f) * 180) + Mathf.Exp(-Mathf.Abs(time - 0.23f) * 140);
                        sample = clicks * (noise * 0.25f + Mathf.Sin(time * 2 * Mathf.PI * 1250) * 0.3f); break;
                    case SoundCue.Step:
                        sample = filtered * Mathf.Exp(-time * 30) * 0.8f + Mathf.Sin(time * 2 * Mathf.PI * 75) * Mathf.Exp(-time * 38) * 0.2f; break;
                    case SoundCue.Impact:
                        sample = noise * Mathf.Exp(-time * 60) * 0.5f + Mathf.Sin(time * 2 * Mathf.PI * 220) * Mathf.Exp(-time * 35) * 0.25f; break;
                    case SoundCue.Zombie:
                        sample = (Mathf.Sin(time * 2 * Mathf.PI * (79 + 5 * Mathf.Sin(time * 17))) * 0.3f + filtered * 0.35f)
                            * Mathf.Sin(Mathf.PI * time / duration); break;
                    default:
                        sample = filtered * Mathf.Exp(-time * 8) + Mathf.Sin(time * 2 * Mathf.PI * 48) * Mathf.Exp(-time * 10) * 0.4f; break;
                }
                data[i] = Mathf.Clamp(sample * Mathf.Min(1, time * 1500), -0.7f, 0.7f);
            }
            var clip = AudioClip.Create("Original synthesized " + cue, data.Length, 1, rate, false);
            clip.SetData(data, 0); return clip;
        }
        private void OnDestroy()
        {
            if (instance == this) instance = null;
            foreach (var clip in clips) if (clip != null) Destroy(clip);
        }
    }
}
