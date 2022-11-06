using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Scriptable object that contains AudioClips that should be synced over network
    [CreateAssetMenu(fileName = "NetworkAudioSync/NetworkAudioClips")]
    public sealed class NetworkAudioClips : ScriptableObject
    {
        // Container for clip entries
        [SerializeField] internal Entry[] registeredClips = Array.Empty<Entry>();

        // True, if this NAC instance is initialized
        [NonSerialized] private bool _clipsInitialized = false;
        
        // NAC instance ID
        private short _id;
        
        
        // Initializes this NAC instance
        public void Initialize()
        {
            if (_clipsInitialized) return;
            _id = NetworkAudioSyncManager.RegisterClips(this);
            _clipsInitialized = true;
        }
        
        // Returns AudioClip by clip ID
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioClip GetAudioClip(int clipHash)
        {
            return NetworkAudioSyncManager.GetAudioClip(_id, clipHash);
        }
        
        // Returns AudioClip by clip name
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioClip GetAudioClip(string clipName)
        {
            int clipHash = NetworkAudioSyncUtils.GetPlatformStableHashCode(clipName);
            return NetworkAudioSyncManager.GetAudioClip(_id, clipHash);
        }

        // Audio clip entry representation
        [Serializable]
        public class Entry
        {
            public string name;
            public AudioClip clip;
        }
    }
}