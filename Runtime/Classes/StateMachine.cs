using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnpopularOpinion {
	public struct StateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable {
		public GameObject Target;
		public StateMachine<T> TargetStateMachine;
		public T NewState;
		public T PreviousState;

		public StateChangeEvent(StateMachine<T> stateMachine) {
			Target = stateMachine.target;
			TargetStateMachine = stateMachine;
			NewState = stateMachine.CurrentState;
			PreviousState = stateMachine.PreviousState;
		}
	}
	[Serializable]
	public class StateMachine<T> : StateMachine where T : struct, IComparable, IConvertible, IFormattable {
		public bool triggerEvents { get; set; }

		public GameObject target { get; protected set; }
		[field: SerializeField, ReadOnlyField]
		public T CurrentState { get; protected set; }
		[field: SerializeField, ReadOnlyField]
		public T PreviousState { get; protected set; }

		public event Action OnChange;
		public event Action<T, T> OnStateChange;

		public StateMachine(GameObject target = null, bool triggerEvents = true) {
			this.target = target;
			this.triggerEvents = triggerEvents;
		}
		public virtual void ChangeState(T newState) {
			// if the "new state" is the current one, we do nothing and exit
			if (EqualityComparer<T>.Default.Equals(newState, CurrentState)) {
				return;
			}

			// we store our previous character movement state
			PreviousState = CurrentState;
			CurrentState = newState;

			OnChange?.Invoke();
			OnStateChange?.Invoke(CurrentState, PreviousState);

			if (triggerEvents) {
				EventManager<StateChangeEvent<T>>.Trigger(new(this));
			}
		}
		public virtual void RestorePreviousState() {
			// we restore our previous state
			CurrentState = PreviousState;

			OnChange?.Invoke();

			if (triggerEvents) {
				EventManager<StateChangeEvent<T>>.Trigger(new(this));
			}
		}
		public override string GetCurrentState() => CurrentState.ToString();
		public override string GetPreviousState() => PreviousState.ToString();
	}
	[Serializable]
	public abstract class StateMachine {
		public abstract string GetCurrentState();
		public abstract string GetPreviousState();
	}
}