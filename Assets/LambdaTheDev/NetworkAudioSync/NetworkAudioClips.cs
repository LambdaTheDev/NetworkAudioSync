using System;
using System.Collections.Generic;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    public class NetworkAudioClips : MonoBehaviour
    {
        //AudioClip is a key, so server has less operations to do
        private readonly Dictionary<AudioClip, byte> _loadedClips = new Dictionary<AudioClip, byte>();
        
        public List<AudioClip> registeredClips;

#if UNITY_EDITOR || UNITY_SERVER

        private void OnValidate()
        {
            if (registeredClips.Count > byte.MaxValue - 1)
            {
                Debug.LogWarning("You can have only 256 registered AudioClips!");
            }
        }

#endif
        
        private void Awake()
        {
            _loadedClips.Clear();
            
            if (registeredClips.Count > byte.MaxValue - 1)
            {
                Debug.LogError("You can have only 256 registered AudioClips!");
                return;
            }
            
            byte nextId = 0;
            foreach (AudioClip clip in registeredClips)
            {
                _loadedClips[clip] = nextId;
                nextId++;
            }
        }

        public byte GetClipId(AudioClip clip)
        {
            return _loadedClips[clip];
        }

        public AudioClip GetAudioClip(byte id)
        {
            foreach (var clip in _loadedClips)
            {
                if (clip.Value == id) return clip.Key;
            }

            return null;
        }
    }
}