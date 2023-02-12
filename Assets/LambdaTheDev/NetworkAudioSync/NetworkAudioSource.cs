using System.Runtime.CompilerServices;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Allows AudioSource settings synchronization over network
    // Base implementation with definitions & essentials
    public partial class NetworkAudioSource : BaseNetworkAudioSyncComponent
    {
        // Audio clips used by this NAS instance
        [SerializeField] private NetworkAudioClips clips;
        

        protected override void VirtualAwake()
        {
            if (clips == null)
                Debug.LogWarning("NetworkAudioClips instance has not been assigned for this NetworkAudioSource. This is not an error, but you will be unable to play other AudioClips than default one!");
            else
                clips.Initialize();

            // Initialize integration
            Integration.BindPacketCallback(OnNetworkPacket);
        }

        protected override void VirtualOnDestroy() { }


        #region Other functionalities

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeClipHashCode(string clipName)
        {
            return NetworkAudioSyncUtils.GetPlatformStableHashCode(clipName);
        }
        
        // Returns state of assigned AudioSource
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private State GetState()
        {
            if (NetworkAudioSyncManager.AudioSourceStates.TryGetValue(AudioSource, out State value))
                return value;

            return null;
        }
        
        // Represents AudioSource state
        internal sealed class State
        {
            public bool IsPaused { get; set; }
        }

        #endregion
    }
}