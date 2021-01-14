using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
namespace LambdaTheDev.NetworkAudioSync
{
    [RequireComponent(typeof(AudioSource))]
    public class NetworkAudioSync : NetworkBehaviour
    {
        public SyncType type;
        public float radius;
        public bool rangeBasedVolume = true;
        public List<AudioClip> registeredClips;

        //AudioClip is a key so the server has
        //less operations to do.
        private Dictionary<AudioClip, int> _parsedAudioClips;
        private AudioSource _source;

        #region Unity events

        private void Start()
        {
            _source = GetComponent<AudioSource>();
            _parsedAudioClips = new Dictionary<AudioClip, int>();

            int id = 1;
            foreach (AudioClip clip in registeredClips)
            {
                _parsedAudioClips.Add(clip, id);
                id++;
            }

            if (rangeBasedVolume)
            {
                _source.rolloffMode = AudioRolloffMode.Linear;
                _source.maxDistance = radius;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void OnValidate()
        {
            if (rangeBasedVolume && type != SyncType.Radius)
            {
                rangeBasedVolume = false;
                Debug.LogError("Range based volume is allowed only with Radius sync type!");
            }
        }

        #endregion

        #region Syncing audio

        [Server]
        public void SyncAudio(AudioClip clip = null)
        {
            int clipToBeSent = 0;

            if (clip != null)
            {
                if (!_parsedAudioClips.TryGetValue(clip, out clipToBeSent))
                {
                    Debug.LogError("Could not get requested AudioClip's ID!");
                    return;
                }
            }

            switch (type)
            {
                case SyncType.Radius:
                    SendToRadius(clipToBeSent, GetScene());
                    break;
                case SyncType.EveryoneInScene:
                    SendToEveryoneInScene(clipToBeSent, GetScene());
                    break;
                // case SyncType.Everyone:
                //     SendToEveryone(clipToBeSent);
                //     break;
            }
        }

        [Server]
        void SendToRadius(int clipId, Scene scene)
        {
            if (radius == 0f)
            {
                Debug.LogError("Radius is 0f!");
                return;
            }

            Collider[] targets = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider target in targets)
            {
                NetworkIdentity identity = target.gameObject.GetComponent<NetworkIdentity>();
                if (identity == null) continue;
                if (identity.gameObject.scene != scene) continue;
                NetworkConnection connection = identity.connectionToClient;
                if(connection == null) continue;
                
                TargetSyncAudio(connection, clipId);
            }
        }

        [Server]
        void SendToEveryoneInScene(int clipId, Scene scene)
        {
            foreach (var connection in NetworkServer.connections)
            {
                NetworkIdentity identity = connection.Value.identity;
                if(identity == null) continue;
                if(identity.gameObject.scene != scene) continue;
                
                TargetSyncAudio(connection.Value, clipId);
            }
        }

        [Server]
        void SendToEveryone(int clipId)
        {
            //Work in progress...
        }

        Scene GetScene()
        {
            return _source.gameObject.scene;
        }

        #endregion
        
        #region Network events

        [TargetRpc]
        void TargetSyncAudio(NetworkConnection connection, int clipId, float volume = 1f)
        {
            _source.volume = volume;
            
            if(clipId == 0) _source.Play();
            else
            {
                AudioClip clip = _parsedAudioClips.FirstOrDefault(x => x.Value == clipId).Key;
                if (clip == null)
                {
                    Debug.LogError("Requested clip with ID: " + clipId + ", but it is not registered!");
                    return;
                }

                _source.clip = clip;
                _source.Play();
            }
        }

        #endregion

        #region Type enum

        public enum SyncType
        {
            Radius, //Send only to players in specific radius (defined in the radius field)
            EveryoneInScene, //Send to every player in a current scene
            //Everyone //Send to everyone on the server (must be DontDestroyOnLoad()) || WORK IN PROGRESS
        }

        #endregion
    }
}
