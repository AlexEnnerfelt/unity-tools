using Unity.Netcode;

namespace UnpopularOpinion.Tools {
    public static class NetcodeUtilities {
        public static bool CanIWrite(this NetworkVariableBase netVar) {
            return netVar.CanClientWrite(NetworkManager.Singleton.LocalClientId);
        }
        public static bool CanIRead(this NetworkVariableBase netVar) {
            return netVar.CanClientRead(NetworkManager.Singleton.LocalClientId);
        }
    }
}
