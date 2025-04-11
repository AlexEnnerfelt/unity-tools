#if NETCODE
using Unity.Netcode;

namespace UnpopularOpinion.Tools {
    public static class NetcodeUtilities {
        public static bool CanIWrite(this NetworkVariableBase netVar) {
            return netVar.CanClientWrite(NetworkManager.Singleton.LocalClientId);
        }
        public static bool CanIRead(this NetworkVariableBase netVar) {
            return netVar.CanClientRead(NetworkManager.Singleton.LocalClientId);
        }
        public static NetworkObject GetNetworkObjectByID(this NetworkManager nm, ulong id)
        {
            nm.SpawnManager.SpawnedObjects.TryGetValue(id, out NetworkObject netObj);
            return netObj;
        }
        public static bool TryGetNetworkObjectByID(this NetworkManager nm, ulong id, out NetworkObject networkObject)
        {
            return nm.SpawnManager.SpawnedObjects.TryGetValue(id, out networkObject);
        }

        public static bool IsUnconnected(this NetworkObject ob)
        {
            return ob.NetworkManager == null || !ob.NetworkManager.IsListening;
        }
    }
}
#endif