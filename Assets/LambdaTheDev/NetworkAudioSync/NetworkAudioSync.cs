using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    public class NetworkAudioSync : NetworkBehaviour
    {
        public AudioSource source;
        public NetworkAudioClips clips;
        
        private void Start()
        {
            if (clips == null)
                throw new NullReferenceException("You haven't provided NetworkAudioClips component for this NetworkAudioSync!");
            
            if (source == null)
                throw new NullReferenceException("You haven't assigned AudioSource for this NetworkAudioSync!");
        }

        [Server]
        public void SyncAudio(AudioClip clip)
        {
            byte id = clips.GetClipId(clip);
            RpcPlayAudio(id);
        }

        [ClientRpc]
        void RpcPlayAudio(byte clipId)
        {
            AudioClip receivedClip = clips.GetAudioClip(clipId);
            source.PlayOneShot(receivedClip);
        }
    }
}