using Newtonsoft.Json.Linq;
using Unity.Netcode;
using UnityEngine;

namespace UnpopularOpinion.TopDown {
    public class ExtendedBehaviour :
#if UNPOP_NETCODE
        NetworkBehaviour
#else
        MonoBehaviour
#endif
        {

#if !UNPOP_NETCODE
        public readonly bool IsOwner = true;
        public readonly bool IsServer = true;
        public readonly bool IsOwnedByServer = true;
        public readonly bool IsLocalPlayer = true;
        public readonly bool IsSpawned = false;
        public readonly ulong LocalClientId = 0;
        public readonly ulong OwnerClientId = 0;
        public NetworkObject NetworkObject => null;
        public NetworkManager NetworkManager => null;
        public ulong NetworkObjectId => 0;
        public bool IsNetcodeActive => false;
        public virtual void OnNetworkSpawn() {
        }
        public virtual void OnNetworkDespawn() { 
        }
        public virtual void OnDestroy()
        {

        }
        protected virtual void OnNetworkPostSpawn()
        {

        }
        protected virtual void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {

        }
#else
        public ulong LocalClientId => NetworkManager.LocalClientId;
        public bool IsNetcodeActive {
            get {

                return NetworkObject != null && NetworkManager != null && NetworkManager.IsListening && NetworkObject.IsSpawned;
            }
        }
#endif

        public const string iconPath = "Assets/Plugins/Unpopular-Topdown/Editor/";
        public const string addComponent = "Unpopular Topdown/";

        public void Print(string message) {
            Debug.Log(message, this);
        }
    }

    public interface IJsonStat {
        public void ApplyStats(JObject jsonObject);
    }
}
