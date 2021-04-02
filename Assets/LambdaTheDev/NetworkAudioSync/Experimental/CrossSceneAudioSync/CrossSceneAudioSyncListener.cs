using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Experimental.CrossSceneAudioSync
{
    [RequireComponent(typeof(AudioSource))]
    public class CrossSceneAudioSyncListener : MonoBehaviour
    {
        public NetworkAudioClips clips;

        private ushort _listenerId;
        private AudioSource _source;
        private CrossSceneAudioSyncInstance _instance;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void LoadListener(ushort id, CrossSceneAudioSyncInstance instance)
        {
            _listenerId = id;
            _instance = instance;
        }

        [Server]
        public void SyncAudio(AudioClip clip)
        {
            byte targetClip = clips.GetClipId(clip);
            _instance.SyncAudioForListener(gameObject.scene.buildIndex, _listenerId, targetClip);
        }

        public void PlayAudio(byte targetClip)
        {
            AudioClip clip = clips.GetAudioClip(targetClip);
            _source.PlayOneShot(clip);
        }
    }
}