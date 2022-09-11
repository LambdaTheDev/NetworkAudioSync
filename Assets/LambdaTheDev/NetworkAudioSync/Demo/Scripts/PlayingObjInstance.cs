using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Demo.Scripts
{
    public class PlayingObjInstance : MonoBehaviour
    {
        private const string ClipName = "TestClip";
        public static PlayingObjInstance Instance { get; private set; }
        
        private NetworkAudioSource _networkAudio;


        private void Awake()
        {
            Instance = this;
            _networkAudio = GetComponent<NetworkAudioSource>();
        }

        public void PlaySound()
        {
            _networkAudio.PlayOneShot(ClipName);
        }
    }
}