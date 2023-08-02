using EtrasStarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace EtrasStarterAssets
{
    public class AudioManager : MonoBehaviour
    {
        public bool silenceSoundsUntilTutorialBegins = true;
        public List<Sound> sounds = new List<Sound>();
        private AudioMixerGroup sfx;
        private AudioMixerGroup music;
        [HideInInspector]public bool stopPlayingSounds = false;

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
                s.source.playOnAwake = false;
                if (s.isMusic)
                {
                    s.source.outputAudioMixerGroup = music;
                }
                else
                {
                    s.source.outputAudioMixerGroup = sfx;
                }

            }
            if (GameObject.Find("NonGamerTutorialManager"))
            {
                if (silenceSoundsUntilTutorialBegins)
                {
                    stopPlayingSounds = true;
                }
            }
        }

        public void Play(string name)
        {
            if (stopPlayingSounds)
            {
                return;
            }

            if (sounds == null)
            {
                Debug.LogWarning("Sound " + name + " not found!");
                return;
            }
            bool soundFound = false;
            foreach (Sound so in sounds)
            {
                if (so.name == name)
                {
                    soundFound = true;
                    break;
                }

            }
            if (soundFound == false)
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
                Debug.LogWarning("Sound " + passedSound.name + " not found!"); // not working?
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

            bool soundFound = false;
            foreach (Sound so in sounds)
            {
                if (so.name == name)
                {
                    soundFound = true;
                    break;
                }

            }
            if (soundFound == false)
            {
                Debug.LogWarning("Sound " + name + " not found!");
                return;
            }
            Sound s = sounds.Find((sound) => sound.name == name);
            s.source.Stop();
        }

        public void stopAllSounds()
        {
            foreach (Sound s in sounds)
            {
                s.source.Stop();
            }
        }
    }
}

