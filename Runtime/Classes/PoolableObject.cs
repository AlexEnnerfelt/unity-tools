using Unity.Netcode;
using UnityEngine.Events;
using UnpopularOpinion.TopDown;

public class PoolableObject : ExtendedBehaviour {
    private NetworkVariable<bool> netIsActivated;
    public bool IsActivated => netIsActivated.Value;

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
    public virtual void Activate() {
        if (netIsActivated.CanClientWrite(LocalClientId)) {
            netIsActivated.Value = true;
            onActivated.Invoke();
        }
    }
    public virtual void Return() {
        if (netIsActivated.CanClientWrite(LocalClientId)) {
            netIsActivated.Value = false;
            onReturned.Invoke();
        }
    }
    protected virtual void OnChangeActivation(bool activation) {
        gameObject.SetActive(activation);
        if (activation) {
            onActivated.Invoke();
        }
        else {

            onReturned.Invoke();
        }
    }
}
