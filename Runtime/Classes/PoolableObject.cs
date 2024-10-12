using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnpopularOpinion.Tools;
using UnpopularOpinion.TopDown;

public class PoolableObject : ExtendedBehaviour {
    private NetworkVariable<bool> netIsActivated;
    public bool IsActivated {
        get {
            if (IsNetcodeActive) {
                return netIsActivated.Value;
            } else {
                return _isActivated;
            }
        }
        set {
            if (IsNetcodeActive && netIsActivated.CanIWrite()) {
                netIsActivated.Value = value;
            }
            _isActivated = value;
            if (value) {
                onActivated.Invoke();
            } else {
                onReturned.Invoke();
            }
        }

    }
    bool _isActivated;
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
        IsActivated = true;
    }
    [ContextMenu("Return")]
    public virtual void Return() {
        IsActivated = false;
    }
    protected virtual void OnChangeActivation(bool activation) {
        if (activation) {
            onActivated.Invoke();
            gameObject.SetActive(activation);

        } else {

            if (disableOnDeactivate) {
                gameObject.SetActive(activation);
            }
            onReturned.Invoke();
        }
    }
}
