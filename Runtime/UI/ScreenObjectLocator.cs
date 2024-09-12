using UnityEngine;
namespace UnpopularOpinion.UICore {
    [ExecuteInEditMode]
    public class ScreenObjectLocator : MonoBehaviour {

        public GameObject target; // The off-screen object
        public GameObject indicator; // The UI element
        public Transform icon;
        public float padding = 50f; // The padding from the edge of the screen

        void LateUpdate() {
            if (target == null) {
                return;
            }
            var targetPos = Camera.main.WorldToScreenPoint(target.transform.position);

            if (targetPos.x < 0 || targetPos.y < 0 || targetPos.x > Screen.width || targetPos.y > Screen.height) {
                // The target is off-screen, so we should display the indicator
                indicator.SetActive(true);

                // We'll just use the x and y coordinates for simplicity
                var onScreenPos = new Vector3(Mathf.Clamp(targetPos.x, padding, Screen.width - padding), Mathf.Clamp(targetPos.y, padding, Screen.height - padding), targetPos.z);

                // Convert the on-screen position back to world space
                var onScreenWorldPos = Camera.main.ScreenToWorldPoint(onScreenPos);

                // Position the indicator at the on-screen position
                indicator.transform.position = onScreenWorldPos;

                // Rotate the indicator to point towards the off-screen target
                var targetDir = target.transform.position - indicator.transform.position;
                var angle = (Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg) - 90; // Subtract 90 to convert from "right = 0" to "up = 0"
                indicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                icon.rotation = Quaternion.identity;
            }
            else {
                // The target is on-screen, so we should hide the indicator
                indicator.SetActive(false);
            }
        }
    }
}