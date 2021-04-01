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

        private void Awake()
        {
            _loadedClips.Clear();
            
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