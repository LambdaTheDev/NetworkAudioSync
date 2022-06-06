using System;
using LambdaTheDev.NetworkAudioSync.InternalNetworking;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LambdaTheDev.NetworkAudioSync.Footsteps
{
    // Small asset, used to synchronize your character's footsteps (it plays random sound after
    //  GameObject moves more than footstepThreshold)
    public class NetworkFootsteps : BaseNetworkAudioSyncComponent
    {
        [SerializeField] private float footstepThreshold = 1.0f;
        [SerializeField] [Range(0, 1f)] private float footstepsVolume = 1.0f;
        [SerializeField] private AudioClip[] footstepSounds = Array.Empty<AudioClip>();
        [SerializeField] private bool footstepsEnabled = true;

        private Vector3 _lastPosition;
        private float _footstepThresholdSquared;


        protected override void VirtualAwake()
        {
            if(footstepSounds.Length == 0)
                Debug.LogError("Footstep sounds array is empty! This will cause errors.");
            
            FootstepThreshold = footstepThreshold;
            Integration.BindPacketCallback(OnNetworkPacket);
        }

        private void Update()
        {
            if (!footstepsEnabled) return;

            Vector3 position = transform.position;
            Vector3 difference = _lastPosition - position;
            if (Vector3.SqrMagnitude(difference) > _footstepThresholdSquared)
            {
                _lastPosition = position;
                AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                AudioSource.PlayOneShot(randomClip, footstepsVolume);
            }
        }

        public float FootstepThreshold
        {
            get => footstepThreshold;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.FootstepThreshold)
                        .WriteFloat(value).Send();
                }
                
                footstepThreshold = value;
                _footstepThresholdSquared = value * value;
            }
        }

        public float FootstepVolume
        {
            get => footstepsVolume;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.FootstepsVolume)
                        .WriteFloat(value).Send();
                }

                footstepsVolume = value;
            }
        }
        
        public bool Enabled
        {
            get => footstepsEnabled;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.FootstepsEnabled)
                        .WriteBool(value).Send();
                }

                footstepsEnabled = value;
            }
        }

        private void OnNetworkPacket(ArraySegment<byte> packet)
        {
            if (Integration.IsServer) return;
            
            using AudioPacketReader reader = NetworkAudioSyncPools.PacketReaderPool.Rent();
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                case AudioSourceActionId.FootstepThreshold:
                    float newThreshold = reader.ReadFloat();
                    footstepThreshold = newThreshold;
                    _footstepThresholdSquared = newThreshold * newThreshold;
                    break;
                
                case AudioSourceActionId.FootstepsEnabled:
                    footstepsEnabled = reader.ReadBool();
                    break;
                
                case AudioSourceActionId.FootstepsVolume:
                    footstepsVolume = reader.ReadFloat();
                    break;

                default:
                    AudioSourceActionId.InvalidPacketThrow();
                    break;
            }
        }
    }
}