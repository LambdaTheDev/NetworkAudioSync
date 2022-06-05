using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Internal NAS components manager
    internal static class NetworkAudioSyncManager
    {
        // Dictionary of dictionaries with AudioClips based on their hash codes
        private static readonly Dictionary<short, Dictionary<int, AudioClip>> RegisteredClips = new Dictionary<short, Dictionary<int, AudioClip>>();

        // Dictionary of all operating AudioSources
        public static readonly Dictionary<AudioSource, NetworkAudioSource.State> AudioSourceStates = new Dictionary<AudioSource, NetworkAudioSource.State>();


        // Checks if this NetworkAudioClips instance is registered in system, if not - it just registers it
        public static short RegisterClips(NetworkAudioClips clips)
        {
            // Ensure that there is no hash code collision
            short clipsHashCode = NetworkAudioSyncUtils.GetPlatformStableHashCodeShort(clips.name);
            if (RegisteredClips.ContainsKey(clipsHashCode))
                throw new DuplicateNameException("NetworkAudioClips ScriptableObject's name caused hash code collision (ScriptableObject name: " + clips.name + "). Please re-name it!");
            // Put AudioClips from SO to internal Dictionary & null string, so it can be GCd
            Dictionary<int, AudioClip> newClips = new Dictionary<int, AudioClip>();
            for (int i = 0; i < clips.registeredClips.Length; i++)
            {
                // Ensure that AudioClips name does not cause collisions
                NetworkAudioClips.Entry entry = clips.registeredClips[i];
                int entryHashCode = NetworkAudioSyncUtils.GetPlatformStableHashCode(entry.name);
                if (newClips.ContainsKey(entryHashCode))
                    throw new DuplicateNameException("AudioClip entry name in NetworkAudioClips caused hash code collision! (ScriptableObject name: " + clips.name + ", Entry name: " + entry.name + "). Please re-name it!");
                
                // If no collision, add AudioClip to dictionary & set name string to null
                newClips.Add(entryHashCode, entry.clip);
                entry.name = null;
            }
            
            // Insert Dictionary with clips to static registry & return NAC instance ID
            RegisteredClips.Add(clipsHashCode, newClips);
            return clipsHashCode;
        }

        // Returns AudioClip for NAC instance with clipsId ID, or null
        public static AudioClip GetAudioClip(short clipsId, int clipNameHash)
        {
            if (RegisteredClips.TryGetValue(clipsId, out var clips))
                if (clips.TryGetValue(clipNameHash, out var clip))
                    return clip;

            return null;
        }
    }
}