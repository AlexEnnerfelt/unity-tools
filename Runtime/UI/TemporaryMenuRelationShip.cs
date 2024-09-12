using System;

namespace UnpopularOpinion.UICore {
	public class TemporaryMenuRelationShip : IDisposable {
		private UIMenu _target;
		private UIMenu _origin;
		private Action _targetOpenAction;
		private Action _targetCloseAction;

		public TemporaryMenuRelationShip(UIMenu target, UIMenu origin, Action onTargetOpenAction, Action onTargetCloseAction) {
			_target = target;
			_origin = origin;
			_targetOpenAction = onTargetOpenAction;
			_targetCloseAction = onTargetCloseAction;
			_origin.OnClose += Close;
			_target.OnClose += End;

		}
		public void Open() {
			_target.Open();
			_targetOpenAction.Invoke();
		}

		private void End() {
			_targetCloseAction.Invoke();
			Dispose();

		}
		public void Close() {
			_target.Close();
			End();
		}

		public void Dispose() {
			_origin.OnClose -= Close;
			_target.OnClose -= End;
		}
	}
	public struct UINavigationEvent {
		public NavigationAction action;
		public string tag;

		private static UINavigationEvent _e;
		private static Action<UINavigationEvent> Event { get; set; }

		public static void AddListener(Action<UINavigationEvent> action) {
			Event += action;
		}
		public static void RemoveListener(Action<UINavigationEvent> action) {
			Event -= action;
		}
		public static void Invoke(NavigationAction action, string tag) {
			_e.action = action;
			_e.tag = tag;
			Event?.Invoke(_e);
		}
	}

	public enum NavigationAction {
		Selected, ValueChanged, Clicked
	}
}