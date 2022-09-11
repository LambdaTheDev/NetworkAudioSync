using LambdaTheDev.NetworkAudioSync.Integrations;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Base class for NetworkAudioSync components
    public abstract class BaseNetworkAudioSyncComponent : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        protected AudioSource AudioSource { get; private set; }
        protected INetworkAudioSyncIntegration Integration { get; private set; }
        
        private void Awake()
        {
            // Initialize AudioSource
            AudioSource = audioSource;
            if (AudioSource == null)
            {
                AudioSource = GetComponent<AudioSource>();
                if(AudioSource == null)
                    throw new MissingComponentException("AudioSource bound to this NetworkAudioSource must be set!");
            }
            
            // Register this AudioSource, if it isn't already
            if (!NetworkAudioSyncManager.AudioSourceStates.ContainsKey(AudioSource))
            {
                NetworkAudioSyncManager.AudioSourceStates.Add(AudioSource, new NetworkAudioSource.State());
                AudioSource.UnPause(); // Ensure that saved state matches real state.
                // Note: Maybe make ApplyState method & make it serializable
            }

            Integration = GetComponent<INetworkAudioSyncIntegration>();
            if (Integration == null)
                throw new MissingComponentException("Could not locate networking integration component! It is required to use NetworkAudioSource functionalities. They are usually named like: NetworkAudioSync(NETWORKING_SOLUTION_NAME)!");
            
            VirtualAwake();
        }

        private void OnDestroy()
        {
            // When object is destroyed, unregister source
            if (AudioSource != null)
                NetworkAudioSyncManager.AudioSourceStates.Remove(AudioSource);
            
            // Terminate integration
            Integration.BindPacketCallback(null);
            
            VirtualOnDestroy();
        }

        protected virtual void VirtualAwake() { }

        protected virtual void VirtualOnDestroy() { }
    }
}