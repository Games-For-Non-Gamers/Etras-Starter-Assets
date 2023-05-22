using EtrasStarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EtrasStarterAssets
{
    public class AudioManager : MonoBehaviour
    {
        public List<Sound> sounds = new List<Sound>();
        private AudioMixerGroup sfx;
        private AudioMixerGroup music;

        //If script added or reset click
        private void Reset()
        {
            sounds = new List<Sound>()
            {
                new Sound()
            };
        }

        void Awake()
        {
            AudioMixer mixer = Resources.Load("StarterAssetsAudioMixer") as AudioMixer;
            music = mixer.FindMatchingGroups("Music")[0];
            sfx = mixer.FindMatchingGroups("SFX")[0];

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.basePitch;
                s.source.loop = s.loop;
                s.source.spatialBlend = s.spatialBlend;

                if (s.isMusic)
                {
                    s.source.outputAudioMixerGroup = music;
                }
                else
                {
                    s.source.outputAudioMixerGroup = sfx;
                }

            }
        }

        public void Play(string name)
        {
            if (sounds == null)
            {
                Debug.LogWarning("Sound " + name + " not found!");
                return;
            }
            Sound s = sounds.Find((sound) => sound.name == name);
            if (s.randomizePitch)
            {
                s.source.pitch = s.basePitch * UnityEngine.Random.Range(s.randomPitchRange.x, s.randomPitchRange.y);
            }

            if (s.source.enabled)
            {
                s.source.Play();
            }
            
        }

        public void Play(Sound passedSound)
        {
            if (sounds == null)
            {
                Debug.LogWarning("Sound " + passedSound.name + " not found!");
                return;
            }
            Sound s = sounds.Find((sound) => sound.name == passedSound.name);
            if (s.randomizePitch)
            {
                s.source.pitch = s.basePitch * UnityEngine.Random.Range(s.randomPitchRange.x, s.randomPitchRange.y);
            }
            s.source.Play();
        }


        public void Stop(string name)
        {
            if (sounds == null)
            {
                Debug.LogWarning("Sound " + name + " not found!");
                return;
            }
            Sound s = sounds.Find((sound) => sound.name == name);
            s.source.Stop();
        }


    }
}

