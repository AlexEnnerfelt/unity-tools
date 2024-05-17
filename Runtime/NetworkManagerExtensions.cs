using System.Linq;
using Unity.Netcode;
using UnityEngine;

public static class NetworkManagerExtensions {
	public static NetworkObject GetNetworkObjectByID(this NetworkManager nm, ulong id) {
		nm.SpawnManager.SpawnedObjects.TryGetValue(id, out NetworkObject netObj);
		return netObj;
	}
	public static bool TryGetNetworkObjectByID(this NetworkManager nm, ulong id, out NetworkObject networkObject) {
		return nm.SpawnManager.SpawnedObjects.TryGetValue(id, out networkObject);
	}

	public static bool IsUnconnected(this NetworkObject ob) {
		return ob.NetworkManager == null || !ob.NetworkManager.IsListening;
	}
}