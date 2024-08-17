using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnpopularOpinion.TopDown;

public class PoolableObject : ExtendedBehaviour {
    private NetworkVariable<bool> netIsActivated;
    public bool IsActivated => netIsActivated.Value;
    public bool disableOnDeactivate = true;

    public UnityEvent onActivated;
    public UnityEvent onReturned;

    public NetworkVariableWritePermission writePermission = NetworkVariableWritePermission.Server;

    public virtual void Awake() {
        netIsActivated = new(writePerm: writePermission);
        netIsActivated.OnValueChanged += (previosValue, newValue) => {
            OnChangeActivation(newValue);
        };
    }
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        Return();
    }
    [ContextMenu("Activate")]
    public virtual void Activate() {
        if (netIsActivated.CanClientWrite(LocalClientId)) {
            netIsActivated.Value = true;
            onActivated.Invoke();
        }
    }
    [ContextMenu("Return")]
    public virtual void Return() {
        if (netIsActivated.CanClientWrite(LocalClientId)) {
            netIsActivated.Value = false;
            onReturned.Invoke();
        }
    }
    protected virtual void OnChangeActivation(bool activation) {
        if (activation) {
            onActivated.Invoke();
            gameObject.SetActive(activation);

        }
        else {

            if (disableOnDeactivate) {
                gameObject.SetActive(activation);
            }
            onReturned.Invoke();
        }
    }
}
