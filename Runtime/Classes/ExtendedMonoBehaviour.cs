using Unity.Netcode;
using UnityEngine;

public class ExtendedMonoBehaviour : MonoBehaviour {
    protected ulong LocalClientId => NetworkManager.Singleton.LocalClientId;
}
