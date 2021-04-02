using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.PreciseSync
{
    [RequireComponent(typeof(NetworkAudioClips))]
    public class NetworkPreciseSync : NetworkBehaviour
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
            byte targetClip = _clips.GetClipId(clip);
            PreciseSyncRequest request = new PreciseSyncRequest
            {
                Clip = targetClip,
                RequestTime = NetworkTime.time
            };
            
            RpcPlayAudio(request);
        }

        [ClientRpc]
        void RpcPlayAudio(PreciseSyncRequest request)
        {
            double offset = NetworkTime.time - request.RequestTime;
            AudioClip clip = _clips.GetAudioClip(request.Clip);

            if (clip == null) return;
            if (clip.length > offset) return;

            source.clip = clip;
            source.time = (float) offset;
            source.Play();
        }
    }
}