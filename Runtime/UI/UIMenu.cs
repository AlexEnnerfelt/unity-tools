using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnpopularOpinion.UICore {
    public abstract class UIMenu : UIDisplay
    {
        private static List<UIMenu> _menus = new();
        public static bool IsMenuUpen {
            get {
                foreach (var item in _menus) {
                    if (item.IsOpen) {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool isFocused;
        public virtual VisualElement FocusOnOpen {
            get => Root;
            set => FocusOnOpen = value;
        }

        public event Action OnClose;
        public event Action OnOpen;
        public event Action OnBlur;
        public event Action OnFocus;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ClearMenuList() {
            _menus.Clear();
        }

        public override void Awake() {
            _menus.Add(this);
            base.Awake();
        }
        public override void Initialization() {
            base.Initialization();
        }
        public virtual void Open() {
            if (!enabled) {
                return;
            }
            IsOpen = true;
            Evaluate();
            OnOpen?.Invoke();
            Focus();
        }
        public virtual void Close() {
            Blur();
            IsOpen = false;
            OnClose?.Invoke();
            Evaluate();
        }

        public virtual void Blur() {
            if (!isFocused) {
                return;
            }
            FocusOnOpen.Blur();
            isFocused = false;
            OnBlur?.Invoke();
        }
        
        private CancellationTokenSource _focusCancellationToken = new();

        public virtual async void Focus() {
            if (isFocused) {
                return;
            }
            isFocused = true;
            _focusCancellationToken = new();
            while (FocusOnOpen.focusable && !FocusOnOpen.canGrabFocus) {
                try {
                    await Awaitable.NextFrameAsync(_focusCancellationToken.Token);
                } catch (OperationCanceledException) {
                    break;
                }
            }
            FocusOnOpen.Focus();
            OnFocus?.Invoke();
            _focusCancellationToken.Cancel();
        }

  
    }
}