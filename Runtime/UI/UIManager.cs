using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UnpopularOpinion.UICore {
	public class UIManager : Singleton<UIManager> {
		[Header("Default Audio")]
		public UIAudio selectedAudio;
		public UIAudio valueChangedAudio;
		public UIAudio clickedAudio;
		public UIAudio[] overrides;
		public override void Awake() {
			base.Awake();
			UINavigationEvent.AddListener(OnNavigation);
			selectedAudio.Initialize();
			valueChangedAudio.Initialize();
			clickedAudio.Initialize();
		}
		public virtual void OnDestroy() {
			UINavigationEvent.RemoveListener(OnNavigation);
		}

		[Serializable]
		public class UIAudio {
			public string tag;
			public float cooldown = 0.2f;
			private float _currentCooldown = 0.2f;
			private bool IsCoolingDown => _currentCooldown < cooldown || AudioTriggeredThisFrame;
			public NavigationAction action;
			public UnityEvent OnTrigger;
			public void Initialize() {
				_currentCooldown = cooldown;
			}
			public void Trigger() {
				if (IsCoolingDown) {
					return;
				}
				StartCooldown();
				OnTrigger.Invoke();
			}
			private async void StartCooldown() {
				GlobalFrameCooldown();
				_currentCooldown = 0;
				while (IsCoolingDown) {
					_currentCooldown += Time.unscaledDeltaTime;
					await Awaitable.NextFrameAsync();
				}
			}

			private static bool AudioTriggeredThisFrame = false;
			private static int frameCooldown = 2;
			private static async void GlobalFrameCooldown() {
				AudioTriggeredThisFrame = true;
				for (var i = 0; i < frameCooldown; i++) {
					await Awaitable.NextFrameAsync();
				}
				AudioTriggeredThisFrame = false;
			}
		}

		public void OnNavigation(UINavigationEvent eventType) {
			if (eventType.tag != null && overrides != null) {
				try {
					var audioOverride = overrides.Where(x => x.tag == eventType.tag && x.action == eventType.action).First();
					if (audioOverride != null) {
						audioOverride.OnTrigger.Invoke();
						return;
					}
				} catch (InvalidOperationException) {

				}
			}
			switch (eventType.action) {
				case NavigationAction.Selected:
					selectedAudio.Trigger();
					break;
				case NavigationAction.ValueChanged:
					valueChangedAudio.Trigger();
					break;
				case NavigationAction.Clicked:
					clickedAudio.Trigger();
					break;
				default:
					selectedAudio.Trigger();
					break;
			}
		}
	}
}