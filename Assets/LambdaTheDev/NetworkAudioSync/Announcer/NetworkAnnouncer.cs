using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Announcer
{
    [RequireComponent(typeof(NetworkAudioClips))]
    public class NetworkAnnouncer : NetworkBehaviour
    {
        private readonly Queue<AudioClip> _pendingAnnouncements = new Queue<AudioClip>();
        
        public AudioSource source;
        
        [Header("Those can be null & don't have to be registered in NetworkAudioClips!")]
        public AudioClip announcementPrefix;
        public AudioClip announcementSuffix;
        public float delayBetweenAnnouncements = 2f;
        public float delayAfterPrefix = 0.5f;

        private NetworkAudioClips _clips;
        private bool _announcing;

        private WaitForSeconds _prefixDelay;
        private WaitForSeconds _suffixDelay;

        private void Start()
        {
            _pendingAnnouncements.Clear();
            _clips = GetComponent<NetworkAudioClips>();
            
            if(announcementPrefix != null) _prefixDelay = new WaitForSeconds(announcementPrefix.length + delayAfterPrefix);
            if(announcementSuffix != null) _suffixDelay = new WaitForSeconds(announcementSuffix.length + delayBetweenAnnouncements);
        }

        [Server]
        public void Announce(AudioClip clip)
        {
            byte id = _clips.GetClipId(clip);

            foreach (var observer in netIdentity.observers)
            {
                TargetEnqueueAnnouncement(observer.Value, id);
            }
        }

        [TargetRpc]
        void TargetEnqueueAnnouncement(NetworkConnection conn, byte targetClip)
        {
            AudioClip receivedClip = _clips.GetAudioClip(targetClip);
            _pendingAnnouncements.Enqueue(receivedClip);
            
            if (!_announcing)
            {
                _announcing = true;
                StartCoroutine(DelayedStartAnnouncing());
            }
        }

        IEnumerator DelayedStartAnnouncing()
        {
            while (_pendingAnnouncements.Count > 0)
            {
                if(announcementPrefix != null) source.PlayOneShot(announcementPrefix);
                yield return _prefixDelay;
                
                AudioClip clip = _pendingAnnouncements.Dequeue();
                source.PlayOneShot(clip);
                yield return new WaitForSeconds(clip.length);
                
                if(announcementSuffix != null) source.PlayOneShot(announcementSuffix);
                yield return _suffixDelay;
            }

            _announcing = false;
        }
    }
}