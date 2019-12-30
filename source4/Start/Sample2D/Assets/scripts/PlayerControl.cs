using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour 
{
	public enum FACEDIRECTION {FACELEFT = -1, FACERIGHT = 1};
	public FACEDIRECTION Facing = FACEDIRECTION.FACERIGHT;

	private Transform ThisTransform = null;
	private Rigidbody2D ThisBody = null;
	public float MaxSpeed = 1f;
	public string HorzAxis = "Horizontal";
	public string JumpButton = "Jump";
	public float JumpPower = 600;
	private bool CanJump = true;
	public float JumpTimeOut = 1f;

	public CircleCollider2D FeetCollider = null;
	public bool isGrounded = false;
	public LayerMask GroundLayer;

	private Animator ThisAnimator = null;
	private int MotionVal = Animator.StringToHash("Motion");
	//--------------------------------
	// Use this for initialization
	void Awake () 
	{
		ThisTransform = GetComponent<Transform>();
		ThisBody = GetComponent<Rigidbody2D>();

		//Get Animator
		ThisAnimator = GetComponent<Animator>();
	}
	//--------------------------------
	//Returns bool - is player on ground?
	private bool GetGrounded()
	{
		//Check ground
		Vector2 CircleCenter = new Vector2(ThisTransform.position.x, ThisTransform.position.y) + FeetCollider.offset;
		Collider2D[] HitColliders = Physics2D.OverlapCircleAll(CircleCenter, FeetCollider.radius, GroundLayer);
		if(HitColliders.Length > 0) return true;
		return false;
	}
	//--------------------------------
	// Update is called once per frame
	void FixedUpdate () 
	{
		//Update grounded status
		isGrounded = GetGrounded();

		float Horz = CrossPlatformInputManager.GetAxis(HorzAxis);
		ThisBody.AddForce(Vector2.right * Horz * MaxSpeed);

		if(CrossPlatformInputManager.GetButton(JumpButton))
			Jump();

		//Clamp velocity
		ThisBody.velocity = new Vector2(Mathf.Clamp(ThisBody.velocity.x, -MaxSpeed, MaxSpeed), 
		                                Mathf.Clamp(ThisBody.velocity.y, -Mathf.Infinity, JumpPower));

		//Flip direction if required
		if((Horz < 0f && Facing != FACEDIRECTION.FACELEFT) || (Horz > 0f && Facing != FACEDIRECTION.FACERIGHT))
			FlipDirection();

		//Update motion Animation
		ThisAnimator.SetFloat(MotionVal, Mathf.Abs(Horz), 0.1f, Time.deltaTime);
	}
	//--------------------------------
	//Engage jump
	private void Jump()
	{
		//If we are grounded, then jump
		if(!isGrounded || !CanJump)return;
		
		//Jump
		ThisBody.AddForce(Vector2.up * JumpPower);
		CanJump = false;
		Invoke ("ActivateJump", JumpTimeOut);
	}
	//--------------------------------
	//Activates can jump variable after jump timeout
	//Prevents double-jumps
	private void ActivateJump()
	{
		CanJump = true;
	}
	//--------------------------------
	//Flips character direction
	private void FlipDirection()
	{
		Facing = (FACEDIRECTION) ((int)Facing * -1f);
		Vector3 LocalScale = ThisTransform.localScale;
		LocalScale.x *= -1f;
		ThisTransform.localScale = LocalScale;
	}
	//--------------------------------
}
