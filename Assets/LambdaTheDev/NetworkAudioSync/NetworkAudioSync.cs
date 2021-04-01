using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    [RequireComponent(typeof(NetworkAudioClips))]
    [RequireComponent(typeof(AudioSource))]
    public class NetworkAudioSync : NetworkBehaviour
    {
        private NetworkAudioClips _clips;
        private AudioSource _source;

        private void Start()
        {
            _clips = GetComponent<NetworkAudioClips>();
            _source = GetComponent<AudioSource>();
        }

        [Server]
        public void SyncAudioClip(AudioClip clip)
        {
            byte id = _clips.GetClipId(clip);

            foreach (var observer in netIdentity.observers)
            {
                TargetSyncAudio(observer.Value, id);
            }
        }

        [TargetRpc]
        public void TargetSyncAudio(NetworkConnection conn, byte clipId)
        {
            AudioClip receivedClip = _clips.GetAudioClip(clipId);
            if (receivedClip == null)
            {
                Debug.LogError("NAS: Received invalid audio sync request!");
                return;
            }
            
            _source.PlayOneShot(receivedClip);
        }
    }
}