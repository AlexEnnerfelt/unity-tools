using UnityEditor;
using UnityEngine;

namespace UnpopularOpinion.Tools {
	/// <summary>
	/// This class adds names for each LevelMapPathElement next to it on the scene view, for easier setup
	/// </summary>
	[CustomEditor(typeof(Path), true)]
	[InitializeOnLoad]
	public class PathEditor : Editor {
		public Path pathTarget {
			get {
				return (Path)target;
			}
		}

		/// <summary>
		/// OnSceneGUI, draws repositionable handles at every point in the path, for easier setup
		/// </summary>
		protected virtual void OnSceneGUI() {
			Handles.color = Color.green;
			var t = target as Path;

			for (var i = 0; i < t.PathElements.Count; i++) {
				EditorGUI.BeginChangeCheck();

				var oldPoint = t.PathElements[i].PathElementPosition;
				var style = new GUIStyle();

				// draws the path item number
				style.normal.textColor = Color.yellow;
				Handles.Label(t.PathElements[i].PathElementPosition + (Vector3.down * 0.4f) + (Vector3.right * 0.4f), "" + i, style);

				// draws a movable handle
				var fmh_49_57_638408331517285186 = Quaternion.identity;
				var newPoint = Handles.FreeMoveHandle(oldPoint, .5f, new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);
				newPoint = ApplyAxisLock(oldPoint, newPoint);

				// records changes
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(target, "Free Move Handle");
					t.PathElements[i].PathElementPosition = newPoint;
				}
			}
		}

		/// <summary>
		/// Locks handles movement on x, y, or z axis
		/// </summary>
		/// <param name="oldPoint"></param>
		/// <param name="newPoint"></param>
		/// <returns></returns>
		protected virtual Vector3 ApplyAxisLock(Vector3 oldPoint, Vector3 newPoint) {
			var t = target as Path;
			if (t.LockHandlesOnXAxis) {
				newPoint.x = oldPoint.x;
			}
			if (t.LockHandlesOnYAxis) {
				newPoint.y = oldPoint.y;
			}
			if (t.LockHandlesOnZAxis) {
				newPoint.z = oldPoint.z;
			}

			return newPoint;
		}
	}
}
