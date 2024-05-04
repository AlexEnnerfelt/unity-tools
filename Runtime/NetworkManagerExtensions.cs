using System.Linq;
using Unity.Netcode;
using UnityEngine;

public static class NetworkManagerExtensions {
	public static NetworkObject GetNetworkObjectByID(this NetworkManager nm, ulong id) {
		var objects = nm.SpawnManager.SpawnedObjectsList;
		foreach (var obj in objects) {
			if (obj.NetworkObjectId == id) {
				return obj;
			}
		}
		return null;
	}
	public static bool TryGetNetworkObjectByID(this NetworkManager nm, ulong id, out NetworkObject networkObject) {
		var objects = nm.SpawnManager.SpawnedObjectsList;
		foreach (var obj in objects) {
			if (obj.NetworkObjectId == id) {
				networkObject = obj;
				return true;
			}
		}
		networkObject = default;
		return false;
	}
	public static NetworkClient GetClientByID(this NetworkManager nm, ulong id = 0) {
		if (!nm.IsServer) {
			if (id != nm.LocalClientId) {
				Debug.LogError("Only the server can access the full client list. Do this server side instead");
				return null;
			} else {
				return nm.LocalClient;
			}
		}
		foreach (var client in nm.ConnectedClientsList) {
			if (client.ClientId == id) {
				return client;
			}
		}
		Debug.LogError($"Couldn't find a NetworkClient with ID: {id}");
		return null;
	}
	public static bool IsUnconnected(this NetworkObject ob) {
		return ob.NetworkManager == null || !ob.NetworkManager.IsListening;
	}
}