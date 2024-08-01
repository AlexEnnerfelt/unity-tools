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
        public const bool IsOwner = true;
        public const bool IsSpawned = false;

#endif
        public ulong LocalClientId => NetworkManager.LocalClientId;
        public bool IsNetcodeActive => NetworkManager != null && NetworkManager.IsListening && NetworkObject.IsSpawned
            ;
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
