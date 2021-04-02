using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.Experimental.CrossSceneAudioSync
{
    public class CrossSceneAudioSyncInstance : NetworkBehaviour
    {
        private static readonly List<CrossSceneAudioSyncListener> PendingListeners = new List<CrossSceneAudioSyncListener>();

        private static CrossSceneAudioSyncInstance _instance;
        
        private readonly Dictionary<int, Dictionary<ushort, CrossSceneAudioSyncListener>> _listeners = new Dictionary<int, Dictionary<ushort, CrossSceneAudioSyncListener>>();
        private readonly Dictionary<int, ushort> _nextIds = new Dictionary<int, ushort>();

        public bool loopManually;

        #region API for manual loops

        public static void LoadObjects(ICollection<CrossSceneAudioSyncListener> listeners)
        {
            if (_instance == null)
            {
                PendingListeners.AddRange(listeners);
                return;
            }

            int sceneId = -1;
            ushort nextListenerId = 0;
            
            foreach (CrossSceneAudioSyncListener listener in listeners)
            {
                if (sceneId == -1)
                {
                    sceneId = listener.gameObject.scene.buildIndex;
                    _instance._nextIds.Add(sceneId, 0);
                }

                _instance._listeners[sceneId][nextListenerId] = listener;
                listener.LoadListener(nextListenerId, _instance);
                nextListenerId++;
            }

            _instance._nextIds[sceneId] = nextListenerId;
        }
        
        #endregion


        #region Unity callbacks

        private void Awake()
        {
            _instance = this;
            _listeners.Clear();
            _nextIds.Clear();

            if (loopManually)
            {
                //todo
                return;
            }

            if (PendingListeners.Count != 0)
            {
                LoadObjects(PendingListeners);
            }
        }

        #endregion


        #region Audio sync

        [Server]
        public void SyncAudioForListener(int sceneId, ushort listener, byte clipId)
        {
            RpcSyncAudio(sceneId, listener, clipId);
        }

        [ClientRpc]
        void RpcSyncAudio(int sceneId, ushort listener, byte clip)
        {
            if (!_instance._listeners.ContainsKey(sceneId)) return;
            if (!_instance._listeners[sceneId].ContainsKey(listener))
            {
                Debug.LogError("Server tries to sync audio to object you do not registered!");
                return;
            }
            
            _instance._listeners[sceneId][listener].PlayAudio(clip);
        }

        #endregion
    }
}