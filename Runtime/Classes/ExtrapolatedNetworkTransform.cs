using System;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Netcode {
	[Icon("Assets/Plugins/Unpopular Topdown/Editor/NetworkTransform Icon.png")]
	public class ExtrapolatedNetworkTransform : NetworkBehaviour {
		[Header("Positional extrapolation")]
		[Space(2)]
		public bool extrapolatePosition;
		public float sendVelocityThreshold = 0.05f;
		public float minimumLerpSpeed = 5f;
		public ExtrapolationSetup setup;
		[ReadOnlyField] public Vector2 networkPosition;
		[ReadOnlyField] public Vector2 networkVelocity;
		[ReadOnlyField] public Vector2 lastNonNullVelocity;
		[ReadOnlyField] public Vector2 extrapolatedPosition;
		[ReadOnlyField] public Vector2 positionError;
		[ReadOnlyField] public float errorMagnitude;
		[ReadOnlyField] public double lastUpdatedServerTime;
		[ReadOnlyField] public float previousVelocityMagnitude;
		[Space]
		[Header("Scale")]
		[Space(2)]
		[ReadOnlyField] public Vector2 networkScale;
		[Space]
		[Header("Network position")]
		[Range(0.1f, 4)]
		public float positionThreshold = 0.1f;

		private bool DoExtrapolate => extrapolatePosition && !IsOwner;
		private double NetworkTime => NetworkManager.Singleton.NetworkTimeSystem.ServerTime;


		private Rigidbody2D _rigidBody2D;
		private Vector2 Velocity => _rigidBody2D.velocity;
		public void Awake() {
			_rigidBody2D = GetComponent<Rigidbody2D>();
		}
		public void FixedUpdate() {
			SendUpdatePosition();
			UpdateScale();
			ExtrapolateMovementFromPreviousData();
		}

		public override void OnNetworkSpawn() {
			base.OnNetworkSpawn();
			if (IsOwner) {
				networkScale = transform.localScale;
			}
			else {
				_rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
			}
			RequestTransformUpdateServerRpc();
		}

		private void UpdateScale(bool forceSend = false) {
			if (!forceSend && !IsOwner) {
				return;
			}
			if (forceSend || IsDifferentlyScaled(transform.localScale, networkScale)) {
				networkScale = transform.localScale;
				HalfVector2 scale = new(transform.localScale.x, transform.localScale.y);
				SendScaleRpc(scale);
			}

			static bool IsDifferentlyScaled(Vector2 scale1, Vector2 scale2) => scale1.x != scale2.x || scale1.y != scale2.y;
		}
		private void SendUpdatePosition(bool forceSend = false) {
			if (!forceSend && (!IsOwner || !extrapolatePosition)) {
				return;
			}
			var velocityMagnitude = Velocity.magnitude;
			if (forceSend || VelocityThresholdMet(velocityMagnitude) || DistanceThresholdMet()) {
				HalfVector2 position = new((half)_rigidBody2D.position.x, (half)_rigidBody2D.position.y);
				HalfVector2 velocity = new((half)Velocity.x, (half)Velocity.y);
				SendPositionServerRpc(position, velocity);
				previousVelocityMagnitude = velocityMagnitude;
			}

			bool DistanceThresholdMet() => Vector2.Distance(transform.position, networkPosition) >= positionThreshold;
			bool VelocityThresholdMet(float velocity) => velocity > sendVelocityThreshold || (velocity == 0 && previousVelocityMagnitude != 0);
		}
		private void ExtrapolateMovementFromPreviousData() {
			if (!DoExtrapolate) {
				return;
			}
			extrapolatedPosition = networkPosition + (networkVelocity * (float)(NetworkTime - lastUpdatedServerTime));
			positionError = extrapolatedPosition - _rigidBody2D.position;
			errorMagnitude = positionError.magnitude;

			//If error magnitude is above the threshold and below the max ceiling, lerp to the extrapolated point. Otherwise, snap to it
			if (errorMagnitude > setup.positionErrorThreshold && errorMagnitude < setup.positionUpperThreshold) {
				//Makes sure that the lerp cannot fall below a certain value
				var lerpSpeed = Mathf.Max(lastNonNullVelocity.magnitude, minimumLerpSpeed);
				_rigidBody2D.position = Vector2.Lerp(_rigidBody2D.position, extrapolatedPosition, minimumLerpSpeed * Time.fixedDeltaTime);
			}
			else {
				_rigidBody2D.position = extrapolatedPosition;
			}
		}

		[Rpc(SendTo.Server)]
		protected void SendScaleRpc(HalfVector2 scale) {
			SendScaleClientRpc(scale);
		}
		[Rpc(SendTo.NotOwner)]
		protected void SendScaleClientRpc(HalfVector2 scale) {
			networkScale = new(scale.x, scale.y);
			transform.localScale = networkScale;
		}

		[Rpc(SendTo.Server)]
		protected void SendPositionServerRpc(HalfVector2 networkPosition, HalfVector2 velocity) {
			this.networkPosition = new(networkPosition.x, networkPosition.y);
			networkVelocity = new(velocity.x, velocity.y);
			SendPositionClientRpc(networkPosition, velocity, NetworkTime);
		}
		[Rpc(SendTo.NotOwner)]
		protected void SendPositionClientRpc(HalfVector2 position, HalfVector2 velocity, double serverTime) {
			networkPosition = new(position.x, position.y);
			networkVelocity = new(velocity.x, velocity.y);

			if (networkVelocity != Vector2.zero) {
				lastNonNullVelocity = networkVelocity;
			}

			lastUpdatedServerTime = serverTime;
		}

		[Rpc(SendTo.Owner)]
		protected void RequestTransformUpdateServerRpc() {
			var forceSendUpdate = true;
			SendUpdatePosition(forceSendUpdate);
			UpdateScale(forceSendUpdate);
		}

		public void OnDrawGizmos() {
			if (DoExtrapolate) {
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(networkPosition, 0.2f);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(transform.position, 0.21f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(extrapolatedPosition, 0.2f);
			}
		}
	}
	[Serializable]
	public struct ExtrapolationSetup {
		[Tooltip("The threshold at which the position starts extrapolating the position")]
		public float positionErrorThreshold;
		[Tooltip("The threshold at which the position stops extrapolating the position")]
		public float positionUpperThreshold;
	}
	public struct HalfVector2 : INetworkSerializeByMemcpy {
		public HalfVector2(float x, float y) {
			this.x = (half)x;
			this.y = (half)y;
		}

		public half x;
		public half y;
	}
}