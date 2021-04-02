using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    [RequireComponent(typeof(NetworkAudioClips))]
    public class NetworkAudioSync : NetworkBehaviour
    {
        public AudioSource source;
        
        private NetworkAudioClips _clips;

        private void Start()
        {
            _clips = GetComponent<NetworkAudioClips>();
        }

        [Server]
        public void SyncAudio(AudioClip clip)
        {
            byte id = _clips.GetClipId(clip);
            RpcPlayAudio(id);
        }

        [ClientRpc]
        void RpcPlayAudio(byte clipId)
        {
            AudioClip receivedClip = _clips.GetAudioClip(clipId);
            if (receivedClip == null)
            {
                Debug.LogError("NAS: Received invalid audio sync request!");
                return;
            }
            
            source.PlayOneShot(receivedClip);
        }
    }
}