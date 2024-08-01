
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace UnpopularOpinion.Tools {
    public class ObjectPool : NetworkBehaviour {
        public PoolableObject objectToPool;
        public int InitialPoolSize = 5;
        public Transform OriginPoint;
        private List<PoolableObject> _pool = new();

        public void InitializePool() {
            for (int i = 0; i < InitialPoolSize; i++) {
                SpawnNewServerRpc();
            }
        }

        private PoolableObject InstantiateNewObject() {
            var obj = Instantiate(objectToPool);

            obj.NetworkObject.SpawnWithOwnership(OwnerClientId);
            obj.NetworkObject.TrySetParent(transform);

            return obj;
        }

        [Rpc(SendTo.Server)]
        private void SpawnNewServerRpc() {
            var obj = InstantiateNewObject();
            _pool.Add(obj);
            AddToPoolRpc(obj.NetworkObjectId);
        }
        [Rpc(SendTo.Owner)]
        private void AddToPoolRpc(ulong objectID) {
            var poolable = NetworkManager.SpawnManager.SpawnedObjects[objectID].GetComponent<PoolableObject>();
            AddObjectToPool(poolable);

        }

        protected virtual void AddObjectToPool(PoolableObject poolable) {
            poolable.gameObject.SetActive(false);
            _pool.Add(poolable);
        }

        private void Update() {
            if (OriginPoint != null) {
                _pool.ForEach(obj => {
                    if (!obj.IsActivated) {
                        obj.transform.position = OriginPoint.position;
                    }
                });
            }
        }

        public async Awaitable<PoolableObject> GetPooledGameObject() {
            if (_pool.Any(obj => !obj.IsActivated)) {
                var readyObject = _pool.First(obj => !obj.IsActivated);
                return readyObject;
            }
            else {
                SpawnNewServerRpc();
                while (!_pool.Any(obj => !obj.IsActivated)) {
                    await Awaitable.NextFrameAsync();
                }
                var readyObject = _pool.First(obj => !obj.IsActivated);
                return readyObject;
            }
        }
    }
}