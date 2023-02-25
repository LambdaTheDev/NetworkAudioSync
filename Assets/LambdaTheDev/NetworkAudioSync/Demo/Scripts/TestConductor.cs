using FishNet;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Demo.Scripts
{
    public class TestConductor : MonoBehaviour
    {
        public bool logPressedKey;
        
        private void Start()
        {
            InstanceFinder.ServerManager.StartConnection(7396);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if(logPressedKey) Debug.Log("Played sound!");
                PlayingObjInstance.Instance.PlaySound();
            }
        }
    }
}