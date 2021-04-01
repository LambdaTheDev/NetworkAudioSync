using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = System.Random;

namespace LambdaTheDev.NetworkAudioSync.FootstepSync
{
    public class NetworkFootsteps : NetworkBehaviour
    {
        private static readonly Random Random = new Random();

        //Set this temporary to false, if your player is crouching or something (SERVER-ONLY)
        public bool SyncFootsteps
        {
            get => _syncFootsteps;
            set { if (isServer) _syncFootsteps = value; }
        }
        
        public List<AudioClip> footstepSounds;
        public AudioSource source;
        public float footstepTreshold = 1f;

        private Vector3 _lastPosition;
        private bool _syncFootsteps;
        private float FootstepTresholdSquared => footstepTreshold * footstepTreshold;

        private void Awake()
        {
            _syncFootsteps = true;
        }

        private void FixedUpdate()
        {
            if(!isServer || !_syncFootsteps) return;

            Vector3 offset = _lastPosition - transform.position;
            if (offset.sqrMagnitude > FootstepTresholdSquared)
            {
                SyncFootstep();
                _lastPosition = transform.position;
            }
        }

        void SyncFootstep()
        {
            foreach (var observer in netIdentity.observers)
            {
                TargetPlayFootstep(observer.Value);
            }
        }

        [TargetRpc(channel = Channels.DefaultUnreliable)]
        void TargetPlayFootstep(NetworkConnection conn)
        {
            int footstepId = Random.Next(footstepSounds.Count);
            AudioClip clip = footstepSounds[footstepId];
            
            source.PlayOneShot(clip);
        }
    }
}