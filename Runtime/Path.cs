using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnpopularOpinion.Tools {
	[Serializable]
	/// <summary>
	/// This class describes a node on an MMPath
	/// </summary>
	public class PathMovementElement {
		/// the point that make up the path the object will follow
		public Vector3 PathElementPosition;
		/// a delay (in seconds) associated to each node
		public float Delay;
	}

	/// <summary>
	/// Add this component to an object and you'll be able to define a path, that can then be used by another component
	/// </summary>
	[AddComponentMenu("Unpopular Opinion/Tools/Path")]
	public class Path : MonoBehaviour, IEnumerator<PathMovementElement> {
		/// the possible cycle options
		public enum CycleOptions {
			BackAndForth,
			Loop,
			OnlyOnce
		}

		public PathMovementElement Current => PathElements[_currentIndex];
		object IEnumerator.Current => Current;

		public bool MoveNext() {
			switch (CycleOption) {
				case CycleOptions.BackAndForth:
					return BackAndForth();
				case CycleOptions.Loop:
					return Loop();
				case CycleOptions.OnlyOnce:
					return OnlyOnce();
				default:
					return OnlyOnce();
			}

			bool BackAndForth() {
				if (Current == GetLast()) {
					ChangeDirection();
					_currentIndex += Direction;
				} else {
					_currentIndex += Direction;
				}
				return true;
			}
			bool Loop() {
				if (Current == GetLast()) {
					_currentIndex = PathElements.IndexOf(GetFirst());
				} else {
					_currentIndex += Direction;
				}
				return true;
			}
			bool OnlyOnce() {
				if (Current == GetLast()) {
					return false;
				} else {
					_currentIndex += Direction;
					return true;
				}
			}
		}

		private PathMovementElement GetLast() {
			if (Direction > 0) {
				return PathElements.Last();
			} else {
				return PathElements.First();
			}
		}
		private PathMovementElement GetFirst() {
			if (Direction > 0) {
				return PathElements.First();
			} else {
				return PathElements.Last();
			}
		}
		public void Reset() {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}

		/// the possible movement directions
		public enum MovementDirection {
			Ascending = 1,
			Descending = -1
		}

		[Header("Path")]
		public CycleOptions CycleOption;

		/// the initial movement direction : ascending > will go from the points 0 to 1, 2, etc ; descending > will go from the last point to last-1, last-2, etc
		public MovementDirection LoopInitialMovementDirection = MovementDirection.Ascending;
		/// the points that make up the path the object will follow
		public List<PathMovementElement> PathElements;
		/// another MMPath that you can reference. If set, the reference MMPath's data will replace this MMPath's
		public Path ReferenceMMPath;
		/// if this is true, this object will move to the 0 position of the reference path
		public bool AbsoluteReferencePath = false;
		/// the minimum distance to a point at which we'll arbitrarily decide the point's been reached
		public float MinDistanceToGoal = .1f;

		[Header("Gizmos")]
		public bool LockHandlesOnXAxis = false;
		public bool LockHandlesOnYAxis = false;
		public bool LockHandlesOnZAxis = false;

		/// if this is true, the object can move along the path
		public virtual bool CanMove { get; set; }
		/// if this is true, this path has gone through its Initialization method
		public virtual bool Initialized { get; set; }

		public virtual int Direction => _direction;
		protected bool _active = false;
		protected IEnumerator<Vector3> _currentPoint;
		protected int _direction = 1;
		protected Vector3 _initialPositionThisFrame;
		protected Vector3 _finalPosition;
		protected Vector3 _previousPoint = Vector3.zero;
		[SerializeField, ReadOnlyField]
		protected int _currentIndex;
		protected float _distanceToNextPoint;
		protected bool _endReached = false;

		protected virtual void Start() {
			if (!Initialized) {
				Initialization();
			}
		}
		public virtual void Initialization() {
			// on Start, we set our active flag to true
			_active = true;
			_endReached = false;
			CanMove = true;

			// we copy our reference if needed
			if ((ReferenceMMPath != null) && (ReferenceMMPath.PathElements != null || ReferenceMMPath.PathElements.Count > 0)) {
				if (AbsoluteReferencePath) {
					this.transform.position = ReferenceMMPath.transform.position;
				}
				PathElements = ReferenceMMPath.PathElements;
			}

			// if the path is null we exit
			if (PathElements == null || PathElements.Count < 1) {
				return;
			}

			// we set our initial direction based on the settings
			if (LoopInitialMovementDirection == MovementDirection.Ascending) {
				_direction = 1;
			} else {
				_direction = -1;
			}

			_currentPoint = GetPathEnumerator();
			_previousPoint = _currentPoint.Current;
			_currentPoint.MoveNext();

		}

		public int CurrentIndex() {
			return _currentIndex;
		}
		public void SetCurrentIndex(int index) {
			_currentIndex = index;
		}
		public Vector3 CurrentPoint() {
			return PathElements[_currentIndex].PathElementPosition;
		}
		public Vector3 GetNextPoint() {
			if (_currentIndex == PathElements.Count) {
				//switch (CycleOption) {
				//	case CycleOptions.BackAndForth:
				//		ChangeDirection();
				//		_currentIndex += _direction;
				//		break;
				//	case CycleOptions.Loop:
				//		SetCurrentIndex(0);
				//		break;
				//	case CycleOptions.OnlyOnce:
				//		break;
				//}
			} else {
				_currentIndex += _direction;
			}
			return PathElements[_currentIndex].PathElementPosition;
		}
		public Vector3 GetLastPoint() {
			return PathElements.Last().PathElementPosition;
		}
		[ContextMenu("ResetPath")]
		public void ResetPath() {
			_currentIndex = 0;
		}

		public virtual int GetIndexOfClosestPoint(Vector2 comparer) {
			var positionAndDistances = new Dictionary<float, int>();
			//Check the distance of all the points
			for (var i = 0; i < PathElements.Count; i++) {
				var distance = Vector2.Distance(comparer, PathElements[i].PathElementPosition);
				positionAndDistances.Add(distance, i);
			}

			var ordered = positionAndDistances.OrderByDescending(kvp => kvp.Key);
			return ordered.First().Value;
		}

		protected virtual void Update() {
			// if the path is null we exit, if we only go once and have reached the end we exit, if we can't move we exit
			if (PathElements == null
			   || PathElements.Count < 1
			   || _endReached
			   || !CanMove
			  ) {
				return;
			}

			ComputePath();
		}
		protected virtual void ComputePath() {
			// we store our initial position to compute the current speed at the end of the udpate	
			_initialPositionThisFrame = this.transform.position;

			// we decide if we've reached our next destination or not, if yes, we move our destination to the next point 
			_distanceToNextPoint = (this.transform.position - _currentPoint.Current).magnitude;
			if (_distanceToNextPoint < MinDistanceToGoal) {
				_previousPoint = _currentPoint.Current;
				_currentPoint.MoveNext();
			}

			// we determine the current speed		
			_finalPosition = this.transform.position;
		}
		public virtual IEnumerator<Vector3> GetPathEnumerator() {

			// if the path is null we exit
			if (PathElements == null || PathElements.Count < 1) {
				yield break;
			}

			var index = 0;
			_currentIndex = index;
			while (true) {
				_currentIndex = index;
				yield return PathElements[index].PathElementPosition;

				if (PathElements.Count <= 1) {
					continue;
				}

				// if the path is looping
				if (CycleOption == CycleOptions.Loop) {
					index = index + _direction;
					if (index < 0) {
						index = PathElements.Count - 1;
					} else if (index > PathElements.Count - 1) {
						index = 0;
					}
				}

				if (CycleOption == CycleOptions.BackAndForth) {
					if (index <= 0) {
						_direction = 1;
					} else if (index >= PathElements.Count - 1) {
						_direction = -1;
					}
					index = index + _direction;
				}

				if (CycleOption == CycleOptions.OnlyOnce) {
					if (index <= 0) {
						_direction = 1;
					} else if (index >= PathElements.Count - 1) {
						_direction = 0;
						_endReached = true;
					}
					index = index + _direction;
				}
			}
		}
		public virtual void ChangeDirection() {
			_direction = -_direction;
			//_currentPoint.MoveNext();
		}
		protected virtual void OnDrawGizmos() {
#if UNITY_EDITOR
			if (PathElements == null) {
				return;
			}

			if (PathElements.Count == 0) {
				return;
			}

			// for each point in the path
			for (var i = 0; i < PathElements.Count; i++) {
				// we draw a green point 
				DebugUtilities.DrawGizmoPoint(PathElements[i].PathElementPosition, 0.2f, Color.green);

				// we draw a line towards the next point in the path
				if ((i + 1) < PathElements.Count) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine(PathElements[i].PathElementPosition, PathElements[i + 1].PathElementPosition);
				}
				// we draw a line from the first to the last point if we're looping
				if ((i == PathElements.Count - 1) && (CycleOption == CycleOptions.Loop)) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine(PathElements[0].PathElementPosition, PathElements[i].PathElementPosition);
				}
			}

			// if the game is playing, we add a blue point to the destination, and a red point to the last visited point
			if (Application.isPlaying) {
				if (_currentPoint != null) {
					DebugUtilities.DrawGizmoPoint(_currentPoint.Current, 0.2f, Color.blue);
					DebugUtilities.DrawGizmoPoint(_previousPoint, 0.2f, Color.red);
				}
			}
#endif


		}


		/// <summary>
		/// A data structure 
		/// </summary>
		[System.Serializable]
		public struct Data {
			public static Data ForwardLoopingPath(Vector3 ctr, Vector3[] vtx, float wait)
				=> new() {
					Center = ctr,
					Offsets = vtx,
					Delay = wait,
					Cycle = CycleOptions.Loop,
					Direction = MovementDirection.Ascending
				};
			public static Data ForwardBackAndForthPath(Vector3 ctr, Vector3[] vtx, float wait)
				=> new() {
					Center = ctr,
					Offsets = vtx,
					Delay = wait,
					Cycle = CycleOptions.BackAndForth,
					Direction = MovementDirection.Ascending
				};
			public static Data ForwardOnlyOncePath(Vector3 ctr, Vector3[] vtx, float wait)
				=> new() {
					Center = ctr,
					Offsets = vtx,
					Delay = wait,
					Cycle = CycleOptions.OnlyOnce,
					Direction = MovementDirection.Ascending
				};

			public Vector3 Center;
			public Vector3[] Offsets;
			public float Delay;
			public CycleOptions Cycle;
			public MovementDirection Direction;
		}

		/// <summary>
		/// Replaces this MMPath's settings with the ones passed in parameters
		/// </summary>
		/// <param name="configuration"></param>
		public void SetPath(in Data configuration) {
			if (configuration.Offsets == null)
				return;

			// same as on Start, we set our active flag to true
			_active = true;
			_endReached = false;
			CanMove = true;

			PathElements = PathElements ?? new List<PathMovementElement>(configuration.Offsets.Length);
			PathElements.Clear();

			foreach (var offset in configuration.Offsets) {
				PathElements.Add(new PathMovementElement() { Delay = configuration.Delay, PathElementPosition = offset });
			}

			// if the path is null we exit
			if (PathElements == null || PathElements.Count < 1) {
				return;
			}

			CycleOption = configuration.Cycle;

			// we set our initial direction based on the settings
			if (configuration.Direction == MovementDirection.Ascending) {
				_direction = 1;
			} else {
				_direction = -1;
			}

			_currentPoint = GetPathEnumerator();
			_previousPoint = _currentPoint.Current;
			_currentPoint.MoveNext();
		}
	}
}