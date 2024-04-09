using System.Collections;
using UnityEngine;

namespace Packages.com.unpopular_opinion.tools.Runtime.Classes {
	public abstract class TagManager : ScriptableObject {
		public static TagManager Instance;
		public void OnEnable() {
			Instance = this;
		}
	}
}