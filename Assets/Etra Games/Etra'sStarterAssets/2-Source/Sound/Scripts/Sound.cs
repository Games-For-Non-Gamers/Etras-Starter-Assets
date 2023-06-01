using UnityEngine;
using UnityEngine.Audio;
namespace EtrasStarterAssets
{
    [System.Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1;
        [Range(0.1f, 3f)]
        public float basePitch = 1;

        [Range(0f, 1f)]
        public float spatialBlend = 1;

        public bool randomizePitch = true;
        public Vector2 randomPitchRange = new Vector2(0.95f, 1.05f);

        public bool isMusic = false;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }
}

