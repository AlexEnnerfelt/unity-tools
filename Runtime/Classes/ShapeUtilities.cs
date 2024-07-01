using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnpopularOpinion.Tools {
	public static class ShapeUtilities {
		public static Vector2 RandomPointInEllipse(float width, float height, Vector2 center) {
			var angle = Random.Range(0f, 2f * Mathf.PI);
			var radius = Mathf.Sqrt(Random.Range(0f, 1f)) * Mathf.Min(width, height) / 2f;
			var x = radius * Mathf.Cos(angle);
			var y = radius * Mathf.Sin(angle);
			return new Vector2(x, y) + center;
		}
		public static Vector2 RandomPointInOval(float width, float height, Vector2 center) {
			var angle = Random.Range(0f, 2f * Mathf.PI);
			var radius = Mathf.Sqrt(Random.Range(0f, 1f)) * Mathf.Min(width, height) / 2f;
			var x = radius * Mathf.Cos(angle);
			var y = radius * Mathf.Sin(angle);
			x *= width / height;
			return new Vector2(x, y) + center;
		}
	}

	public static class SceneUtilities {
		/// <summary>
		/// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
		/// </summary>
		public static bool DoesSceneExist(string name) {
			if (string.IsNullOrEmpty(name))
				return false;

			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
				var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
				var lastSlash = scenePath.LastIndexOf("/");
				var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

				if (string.Compare(name, sceneName, true) == 0)
					return true;
			}

			return false;
		}
	}
}