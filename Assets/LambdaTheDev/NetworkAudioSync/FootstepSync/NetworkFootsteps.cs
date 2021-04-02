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

        public List<AudioClip> footstepSounds;
        public AudioSource source;
        public float footstepTreshold = 1f;
        private float FootstepTresholdSquared => footstepTreshold * footstepTreshold;
        
        //Set this temporary to false, if your player is crouching or something (SERVER-ONLY)
        public bool SyncFootsteps
        {
            get => _syncFootsteps;
            set { if (isServer) _syncFootsteps = value; }
        }
        
        private bool _syncFootsteps = true;

        private Vector3 _lastPosition;
        private bool _disabled;

        private void Start()
        {
            if (source == null)
                throw new NullReferenceException("You haven't assigned AudioSource for this NetworkFootsteps!");

            if (footstepSounds.Count == 0)
            {
                Debug.LogWarning("You haven't registered any footsteps clips. Module is being disabled...");
                _disabled = true;
            }
        }

        private void FixedUpdate()
        {
            if(!isServer || !_syncFootsteps || _disabled) return;

            Vector3 offset = _lastPosition - transform.position;
            if (offset.sqrMagnitude > FootstepTresholdSquared)
            {
                RpcPlayFootstep();
                _lastPosition = transform.position;
            }
        }
        

        [ClientRpc(channel = Channels.DefaultUnreliable)]
        void RpcPlayFootstep()
        {
            int footstepId = Random.Next(footstepSounds.Count);
            AudioClip clip = footstepSounds[footstepId];
            
            source.PlayOneShot(clip);
        }
    }
}