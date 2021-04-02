using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
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
            if (registeredClips.Count - 1 > byte.MaxValue)
            {
                Debug.LogError("You can register up to 256 AudioClips in single NetworkAudioClips component!");
            }
        }

#endif
        
        private void Awake()
        {
            _loadedClips.Clear();
            
            if (registeredClips.Count - 1 > byte.MaxValue)
            {
                throw new IndexOutOfRangeException("You can register up to 256 AudioClips in single NetworkAudioClips component!");
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
            if (!_loadedClips.ContainsKey(clip))
            {
                throw new InvalidDataException("Tried to get ID of unregistered AudioClip!");
            }

            return _loadedClips[clip];
        }

        public AudioClip GetAudioClip(byte id)
        {
            foreach (var clip in _loadedClips)
            {
                if (clip.Value == id) return clip.Key;
            }

            throw new InvalidDataException("Could not get AudioClip for incoming ID: " + id + "!");
        }
    }
}