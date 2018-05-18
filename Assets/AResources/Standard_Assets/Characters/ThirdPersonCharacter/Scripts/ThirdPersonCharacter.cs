using UnityEngine;
namespace UnityStandardAssets.Characters.ThirdPerson {
    [RequireComponent (typeof (Rigidbody))]
	[RequireComponent (typeof (CapsuleCollider))]
	[RequireComponent (typeof (Animator))]
	public class ThirdPersonCharacter : MonoBehaviour {
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range (1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		float m_GroundCheckDistance = 0.01f;
		Rigidbody m_Rigidbody;
		Animator m_Animator;
		public bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;
		Vector3 gravityDrct;
		void Start () {
			m_Animator = GetComponent<Animator> ();
			m_Rigidbody = GetComponent<Rigidbody> ();
			m_Capsule = GetComponent<CapsuleCollider> ();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_GroundCheckDistance = 0.01f;// gameObject.GetComponent<CapsuleCollider>().height * 0.05f * transform.localScale.x;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

		public void Move (Vector3 move, bool crouch, bool jump) {

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize ();
			if (move.magnitude > 0.1f) {
				//Debug.Log ("world space move direction:" + move);
			}
            //Debug.Log ("wolrd move:" + move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal).normalized;
            move = transform.InverseTransformDirection (move);
            //move = Quaternion.FromToRotation (transform.up, Vector3.up) * move;
            // if (move.magnitude > 0.1f) {
            // 	Debug.Log ("local space move direction:" + move);
            // }

            //Debug.Log ("local move:" + move);
            // if (move.magnitude > 0.1f) {
            // 	Debug.Log ("ground normal:" + m_GroundNormal);
            // }

            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;
            ApplyExtraTurnRotation();
            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded) {
                

                HandleGroundedMovement (crouch, jump);
			} else {
				HandleAirborneMovement ();
			}

			ScaleCapsuleForCrouching (crouch);
			PreventStandingInLowHeadroom ();

			// send input and other state parameters to the animator
			UpdateAnimator (move);
		}

		void ScaleCapsuleForCrouching (bool crouch) {
			if (m_IsGrounded && crouch) {
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			} else {
				Ray crouchRay = new Ray (m_Rigidbody.position + transform.up * m_Capsule.radius * transform.localScale.y * k_Half, transform.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                crouchRayLength *= transform.localScale.y;
                Debug.DrawLine(crouchRay.origin, crouchRay.origin + crouchRay.direction * crouchRayLength,Color.green);

                if (Physics.SphereCast (crouchRay, m_Capsule.radius * transform.localScale.x * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
					//Debug.Log ("dun");
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom () {
			// prevent standing up in crouch-only zones
			if (!m_Crouching) {
				Ray crouchRay = new Ray (m_Rigidbody.position + transform.up * m_Capsule.radius * transform.localScale.y * k_Half, transform.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                crouchRayLength *= transform.localScale.y;
                if (Physics.SphereCast (crouchRay, m_Capsule.radius * transform.localScale.x * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
					//ebug.Log ("dun"); * transform.localScale.x 
					m_Crouching = true;
				}
			}
		}

		void UpdateAnimator (Vector3 move) {
			// update the animator parameters
			m_Animator.SetFloat ("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat ("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool ("Crouch", m_Crouching);
			m_Animator.SetBool ("OnGround", m_IsGrounded);
			if (!m_IsGrounded) {
				m_Animator.SetFloat ("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat (
					m_Animator.GetCurrentAnimatorStateInfo (0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded) {
				m_Animator.SetFloat ("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0) {
				m_Animator.speed = m_AnimSpeedMultiplier;
			} else {
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}

		void HandleAirborneMovement () {
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce (extraGravityForce);
			gravityDrct = -this.gameObject.transform.up;
			m_GroundCheckDistance = Vector3.Dot (m_Rigidbody.velocity, Vector3.Normalize (gravityDrct)) > 0.1 ? m_OrigGroundCheckDistance : 0.01f;
            //m_GroundCheckDistance = 0.01f;
        }

		void HandleGroundedMovement (bool crouch, bool jump) {
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo (0).IsName ("Grounded")) {
				// jump!
				m_Rigidbody.velocity = new Vector3 (m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z) + transform.up * m_JumpPower;
				//mata add "m_Rigidbody.velocity.y "
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.01f;
			}
		}

		void ApplyExtraTurnRotation () {
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp (m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate (0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
			//transform.RotateAround (transform.position, transform.up, m_TurnAmount * turnSpeed * Time.deltaTime);
		}

		public void OnAnimatorMove () {
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0) {
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				//v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}

		void CheckGroundStatus () {
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine (transform.position + (this.gameObject.transform.up * m_GroundCheckDistance * 1f), transform.position - this.gameObject.transform.up * m_GroundCheckDistance,Color.cyan);
#endif

			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast (transform.position + (this.gameObject.transform.up * 0.01f), Physics.gravity, out hitInfo, m_GroundCheckDistance * 1f + 0.01f)) {

				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			} else {

				m_IsGrounded = false;
				m_GroundNormal = this.gameObject.transform.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}