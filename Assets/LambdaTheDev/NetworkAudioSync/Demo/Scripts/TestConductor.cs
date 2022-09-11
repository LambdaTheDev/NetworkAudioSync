using System;
using FishNet;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Demo.Scripts
{
    public class TestConductor : MonoBehaviour
    {
        private void Start()
        {
            InstanceFinder.ServerManager.StartConnection();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
                PlayingObjInstance.Instance.PlaySound();
        }
    }
}