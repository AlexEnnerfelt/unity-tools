using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnpopularOpinion {
	public class DebugUtilities {
		#region DebugDraw

		/// <summary>
		/// Draws a gizmo arrow going from the origin position and along the direction Vector3
		/// </summary>
		/// <param name="origin">Origin.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="color">Color.</param>
		public static void DrawGizmoArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 3f, float arrowHeadAngle = 25f) {
			Gizmos.color = color;
			Gizmos.DrawRay(origin, direction);

			DrawArrowEnd(true, origin, direction, color, arrowHeadLength, arrowHeadAngle);
		}

		/// <summary>
		/// Draws a debug arrow going from the origin position and along the direction Vector3
		/// </summary>
		/// <param name="origin">Origin.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="color">Color.</param>
		public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 0.2f, float arrowHeadAngle = 35f) {
			Debug.DrawRay(origin, direction, color);

			DrawArrowEnd(false, origin, direction, color, arrowHeadLength, arrowHeadAngle);
		}

		/// <summary>
		/// Draws a debug arrow going from the origin position and along the direction Vector3
		/// </summary>
		/// <param name="origin">Origin.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="color">Color.</param>
		/// <param name="arrowLength">Arrow length.</param>
		/// <param name="arrowHeadLength">Arrow head length.</param>
		/// <param name="arrowHeadAngle">Arrow head angle.</param>
		public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowLength, float arrowHeadLength = 0.20f, float arrowHeadAngle = 35.0f) {
			Debug.DrawRay(origin, direction * arrowLength, color);

			DrawArrowEnd(false, origin, direction * arrowLength, color, arrowHeadLength, arrowHeadAngle);
		}

		/// <summary>
		/// Draws a debug cross of the specified size and color at the specified point
		/// </summary>
		/// <param name="spot">Spot.</param>
		/// <param name="crossSize">Cross size.</param>
		/// <param name="color">Color.</param>
		public static void DebugDrawCross(Vector3 spot, float crossSize, Color color) {
			var tempOrigin = Vector3.zero;
			var tempDirection = Vector3.zero;

			tempOrigin.x = spot.x - (crossSize / 2);
			tempOrigin.y = spot.y - (crossSize / 2);
			tempOrigin.z = spot.z;
			tempDirection.x = 1;
			tempDirection.y = 1;
			tempDirection.z = 0;
			Debug.DrawRay(tempOrigin, tempDirection * crossSize, color);

			tempOrigin.x = spot.x - (crossSize / 2);
			tempOrigin.y = spot.y + (crossSize / 2);
			tempOrigin.z = spot.z;
			tempDirection.x = 1;
			tempDirection.y = -1;
			tempDirection.z = 0;
			Debug.DrawRay(tempOrigin, tempDirection * crossSize, color);
		}

		/// <summary>
		/// Draws the arrow end for DebugDrawArrow
		/// </summary>
		/// <param name="drawGizmos">If set to <c>true</c> draw gizmos.</param>
		/// <param name="arrowEndPosition">Arrow end position.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="color">Color.</param>
		/// <param name="arrowHeadLength">Arrow head length.</param>
		/// <param name="arrowHeadAngle">Arrow head angle.</param>
		private static void DrawArrowEnd(bool drawGizmos, Vector3 arrowEndPosition, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 40.0f) {
			if (direction == Vector3.zero) {
				return;
			}
			var right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
			var left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
			var up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
			var down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
			if (drawGizmos) {
				Gizmos.color = color;
				Gizmos.DrawRay(arrowEndPosition + direction, right * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, left * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, up * arrowHeadLength);
				Gizmos.DrawRay(arrowEndPosition + direction, down * arrowHeadLength);
			} else {
				Debug.DrawRay(arrowEndPosition + direction, right * arrowHeadLength, color);
				Debug.DrawRay(arrowEndPosition + direction, left * arrowHeadLength, color);
				Debug.DrawRay(arrowEndPosition + direction, up * arrowHeadLength, color);
				Debug.DrawRay(arrowEndPosition + direction, down * arrowHeadLength, color);
			}
		}

		/// <summary>
		/// Draws handles to materialize the bounds of an object on screen.
		/// </summary>
		/// <param name="bounds">Bounds.</param>
		/// <param name="color">Color.</param>
		public static void DrawHandlesBounds(Bounds bounds, Color color) {
#if UNITY_EDITOR
			var boundsCenter = bounds.center;
			var boundsExtents = bounds.extents;

			var v3FrontTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top left corner
			var v3FrontTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front top right corner
			var v3FrontBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom left corner
			var v3FrontBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z - boundsExtents.z);  // Front bottom right corner
			var v3BackTopLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top left corner
			var v3BackTopRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y + boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back top right corner
			var v3BackBottomLeft = new Vector3(boundsCenter.x - boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom left corner
			var v3BackBottomRight = new Vector3(boundsCenter.x + boundsExtents.x, boundsCenter.y - boundsExtents.y, boundsCenter.z + boundsExtents.z);  // Back bottom right corner


			Handles.color = color;

			Handles.DrawLine(v3FrontTopLeft, v3FrontTopRight);
			Handles.DrawLine(v3FrontTopRight, v3FrontBottomRight);
			Handles.DrawLine(v3FrontBottomRight, v3FrontBottomLeft);
			Handles.DrawLine(v3FrontBottomLeft, v3FrontTopLeft);

			Handles.DrawLine(v3BackTopLeft, v3BackTopRight);
			Handles.DrawLine(v3BackTopRight, v3BackBottomRight);
			Handles.DrawLine(v3BackBottomRight, v3BackBottomLeft);
			Handles.DrawLine(v3BackBottomLeft, v3BackTopLeft);

			Handles.DrawLine(v3FrontTopLeft, v3BackTopLeft);
			Handles.DrawLine(v3FrontTopRight, v3BackTopRight);
			Handles.DrawLine(v3FrontBottomRight, v3BackBottomRight);
			Handles.DrawLine(v3FrontBottomLeft, v3BackBottomLeft);
#endif
		}

		/// <summary>
		/// Draws a solid rectangle at the specified position and size, and of the specified colors
		/// </summary>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="borderColor"></param>
		/// <param name="solidColor"></param>
		public static void DrawSolidRectangle(Vector3 position, Vector3 size, Color borderColor, Color solidColor) {
#if UNITY_EDITOR

			var halfSize = size / 2f;

			var verts = new Vector3[4];
			verts[0] = new Vector3(halfSize.x, halfSize.y, halfSize.z);
			verts[1] = new Vector3(-halfSize.x, halfSize.y, halfSize.z);
			verts[2] = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
			verts[3] = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
			Handles.DrawSolidRectangleWithOutline(verts, solidColor, borderColor);

#endif
		}

		/// <summary>
		/// Draws a gizmo sphere of the specified size and color at a position
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="size">Size.</param>
		/// <param name="color">Color.</param>
		public static void DrawGizmoPoint(Vector3 position, float size, Color color) {
			Gizmos.color = color;
			Gizmos.DrawWireSphere(position, size);
		}

		/// <summary>
		/// Draws a cube at the specified position, and of the specified color and size
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		public static void DrawCube(Vector3 position, Color color, Vector3 size) {
			var halfSize = size / 2f;

			var points = new Vector3[]
			{
				position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
				position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
				position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
				position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
				position + new Vector3(halfSize.x,halfSize.y,-halfSize.z),
				position + new Vector3(-halfSize.x,halfSize.y,-halfSize.z),
				position + new Vector3(-halfSize.x,-halfSize.y,-halfSize.z),
				position + new Vector3(halfSize.x,-halfSize.y,-halfSize.z),
			};

			Debug.DrawLine(points[0], points[1], color);
			Debug.DrawLine(points[1], points[2], color);
			Debug.DrawLine(points[2], points[3], color);
			Debug.DrawLine(points[3], points[0], color);
		}

		/// <summary>
		/// Draws a cube at the specified position, offset, and of the specified size
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="offset"></param>
		/// <param name="cubeSize"></param>
		/// <param name="wireOnly"></param>
		public static void DrawGizmoCube(Transform transform, Vector3 offset, Vector3 cubeSize, bool wireOnly) {
			var rotationMatrix = transform.localToWorldMatrix;
			Gizmos.matrix = rotationMatrix;
			if (wireOnly) {
				Gizmos.DrawWireCube(offset, cubeSize);
			} else {
				Gizmos.DrawCube(offset, cubeSize);
			}
		}

		/// <summary>
		/// Draws a gizmo rectangle
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="size">Size.</param>
		/// <param name="color">Color.</param>
		public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Color color) {
			Gizmos.color = color;

			var v3TopLeft = new Vector3(center.x - (size.x / 2), center.y + (size.y / 2), 0);
			var v3TopRight = new Vector3(center.x + (size.x / 2), center.y + (size.y / 2), 0);
			;
			var v3BottomRight = new Vector3(center.x + (size.x / 2), center.y - (size.y / 2), 0);
			;
			var v3BottomLeft = new Vector3(center.x - (size.x / 2), center.y - (size.y / 2), 0);
			;

			Gizmos.DrawLine(v3TopLeft, v3TopRight);
			Gizmos.DrawLine(v3TopRight, v3BottomRight);
			Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
			Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
		}

		/// <summary>
		/// Draws a gizmo rectangle
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="size">Size.</param>
		/// <param name="color">Color.</param>
		public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Matrix4x4 rotationMatrix, Color color) {
			GL.PushMatrix();

			Gizmos.color = color;

			Vector3 v3TopLeft = rotationMatrix * new Vector3(center.x - (size.x / 2), center.y + (size.y / 2), 0);
			Vector3 v3TopRight = rotationMatrix * new Vector3(center.x + (size.x / 2), center.y + (size.y / 2), 0);
			;
			Vector3 v3BottomRight = rotationMatrix * new Vector3(center.x + (size.x / 2), center.y - (size.y / 2), 0);
			;
			Vector3 v3BottomLeft = rotationMatrix * new Vector3(center.x - (size.x / 2), center.y - (size.y / 2), 0);
			;


			Gizmos.DrawLine(v3TopLeft, v3TopRight);
			Gizmos.DrawLine(v3TopRight, v3BottomRight);
			Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
			Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
			GL.PopMatrix();
		}

		/// <summary>
		/// Draws a rectangle based on a Rect and color
		/// </summary>
		/// <param name="rectangle">Rectangle.</param>
		/// <param name="color">Color.</param>
		public static void DrawRectangle(Rect rectangle, Color color) {

			var pos = new Vector3(rectangle.x + (rectangle.width / 2), rectangle.y + (rectangle.height / 2), 0.0f);
			var scale = new Vector3(rectangle.width, rectangle.height, 0.0f);

			DrawRectangle(pos, color, scale);
		}

		/// <summary>
		/// Draws a rectangle of the specified color and size at the specified position
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		public static void DrawRectangle(Vector3 position, Color color, Vector3 size) {
			var halfSize = size / 2f;

			var points = new Vector3[]
			{
				position + new Vector3(halfSize.x,halfSize.y,halfSize.z),
				position + new Vector3(-halfSize.x,halfSize.y,halfSize.z),
				position + new Vector3(-halfSize.x,-halfSize.y,halfSize.z),
				position + new Vector3(halfSize.x,-halfSize.y,halfSize.z),
			};

			Debug.DrawLine(points[0], points[1], color);
			Debug.DrawLine(points[1], points[2], color);
			Debug.DrawLine(points[2], points[3], color);
			Debug.DrawLine(points[3], points[0], color);
		}

		/// <summary>
		/// Draws a point of the specified color and size at the specified position
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="col">Col.</param>
		/// <param name="scale">Scale.</param>
		public static void DrawPoint(Vector3 position, Color color, float size) {
			var points = new Vector3[]
			{
				position + (Vector3.up * size),
				position - (Vector3.up * size),
				position + (Vector3.right * size),
				position - (Vector3.right * size),
				position + (Vector3.forward * size),
				position - (Vector3.forward * size)
			};

			Debug.DrawLine(points[0], points[1], color);
			Debug.DrawLine(points[2], points[3], color);
			Debug.DrawLine(points[4], points[5], color);
			Debug.DrawLine(points[0], points[2], color);
			Debug.DrawLine(points[0], points[3], color);
			Debug.DrawLine(points[0], points[4], color);
			Debug.DrawLine(points[0], points[5], color);
			Debug.DrawLine(points[1], points[2], color);
			Debug.DrawLine(points[1], points[3], color);
			Debug.DrawLine(points[1], points[4], color);
			Debug.DrawLine(points[1], points[5], color);
			Debug.DrawLine(points[4], points[2], color);
			Debug.DrawLine(points[4], points[3], color);
			Debug.DrawLine(points[5], points[2], color);
			Debug.DrawLine(points[5], points[3], color);
		}

		/// <summary>
		/// Draws a line of the specified color and size using gizmos
		/// </summary>
		/// <param name="position"></param>
		/// <param name="color"></param>
		/// <param name="size"></param>
		public static void DrawGizmoPoint(Vector3 position, Color color, float size) {
			var points = new Vector3[]
			{
				position + (Vector3.up * size),
				position - (Vector3.up * size),
				position + (Vector3.right * size),
				position - (Vector3.right * size),
				position + (Vector3.forward * size),
				position - (Vector3.forward * size)
			};

			Gizmos.color = color;
			Gizmos.DrawLine(points[0], points[1]);
			Gizmos.DrawLine(points[2], points[3]);
			Gizmos.DrawLine(points[4], points[5]);
			Gizmos.DrawLine(points[0], points[2]);
			Gizmos.DrawLine(points[0], points[3]);
			Gizmos.DrawLine(points[0], points[4]);
			Gizmos.DrawLine(points[0], points[5]);
			Gizmos.DrawLine(points[1], points[2]);
			Gizmos.DrawLine(points[1], points[3]);
			Gizmos.DrawLine(points[1], points[4]);
			Gizmos.DrawLine(points[1], points[5]);
			Gizmos.DrawLine(points[4], points[2]);
			Gizmos.DrawLine(points[4], points[3]);
			Gizmos.DrawLine(points[5], points[2]);
			Gizmos.DrawLine(points[5], points[3]);
		}

		#endregion

		#region Info

		public static string GetSystemInfo() {
			var result = "SYSTEM INFO";

#if UNITY_IOS
                 result += "\n[iPhone generation]iPhone.generation.ToString()";
#endif

#if UNITY_ANDROID
                result += "\n[system info]" + SystemInfo.deviceModel;
#endif

			result += "\n<color=#FFFFFF>Device Type :</color> " + SystemInfo.deviceType;
			result += "\n<color=#FFFFFF>OS Version :</color> " + SystemInfo.operatingSystem;
			result += "\n<color=#FFFFFF>System Memory Size :</color> " + SystemInfo.systemMemorySize;
			result += "\n<color=#FFFFFF>Graphic Device Name :</color> " + SystemInfo.graphicsDeviceName + " (version " + SystemInfo.graphicsDeviceVersion + ")";
			result += "\n<color=#FFFFFF>Graphic Memory Size :</color> " + SystemInfo.graphicsMemorySize;
			result += "\n<color=#FFFFFF>Graphic Max Texture Size :</color> " + SystemInfo.maxTextureSize;
			result += "\n<color=#FFFFFF>Graphic Shader Level :</color> " + SystemInfo.graphicsShaderLevel;
			result += "\n<color=#FFFFFF>Compute Shader Support :</color> " + SystemInfo.supportsComputeShaders;

			result += "\n<color=#FFFFFF>Processor Count :</color> " + SystemInfo.processorCount;
			result += "\n<color=#FFFFFF>Processor Type :</color> " + SystemInfo.processorType;
			result += "\n<color=#FFFFFF>3D Texture Support :</color> " + SystemInfo.supports3DTextures;
			result += "\n<color=#FFFFFF>Shadow Support :</color> " + SystemInfo.supportsShadows;

			result += "\n<color=#FFFFFF>Platform :</color> " + Application.platform;
			result += "\n<color=#FFFFFF>Screen Size :</color> " + Screen.width + " x " + Screen.height;
			result += "\n<color=#FFFFFF>DPI :</color> " + Screen.dpi;

			return result;
		}

		#endregion
	}
}

