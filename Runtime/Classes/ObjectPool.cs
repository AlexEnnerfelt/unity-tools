
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace UnpopularOpinion.Tools {
    public class ObjectPool : NetworkBehaviour {
        public PoolableObject objectToPool;
        public int InitialPoolSize = 5;

        protected List<PoolableObject> _pool = new();

        public void InitializePool() {
            for (int i = 0; i < InitialPoolSize; i++) {
                SpawnNewServerRpc();
            }
        }
        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            if (IsServer) {
                _pool.ForEach(poolable => {
                    if (poolable.IsSpawned) {
                        poolable.NetworkObject.Despawn();
                    }
                    else {
                        Destroy(poolable.gameObject);
                    }
                });
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