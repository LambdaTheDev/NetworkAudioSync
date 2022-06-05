using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LambdaTheDev.NetworkAudioSync.Footsteps
{
    // Small asset, used to synchronize your character's footsteps (it plays random sound after
    //  GameObject moves more than footstepThreshold)
    public class NetworkFootsteps : MonoBehaviour
    {
        public AudioSource source;
        public float footstepThreshold;
        public AudioClip[] footstepSounds = Array.Empty<AudioClip>();
        [Range(0, 1)] public float footstepsVolume;

        private Vector3 _lastPosition;
        
        private void Start()
        {
            if(footstepSounds.Length == 0)
                Debug.LogError("Footstep sounds are empty! This will cause errors.");
        }

        private void Update()
        {
            Vector3 position = transform.position;
            Vector3 difference = position - _lastPosition;
            if (Vector3.SqrMagnitude(difference) > footstepThreshold * footstepThreshold)
            {
                _lastPosition = position;
                int randomSound = Random.Range(0, footstepSounds.Length);
                source.PlayOneShot(footstepSounds[randomSound], footstepsVolume);
            }
        }
    }
}