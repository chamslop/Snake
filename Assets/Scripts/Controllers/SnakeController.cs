using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Managers;

public class SnakeController : MonoBehaviour {
	// Components
	private Animator anim;
	private Rigidbody2D rb2d;
	private GameObject baseMiddle;
	private GameObject tail;

	// Internals
	private const float defaultSpeed = 10000f;
	private float speed = defaultSpeed;

	void Start() {
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		
		baseMiddle = GameObject.Find("Middle");
		tail = GameObject.Find("Tail");

		EventManager.OnPointScored += speedUp;

		rb2d.AddForce(new Vector2(0, -1 * defaultSpeed * Time.fixedDeltaTime));
	}
	
	void FixedUpdate() {
		Vector3 headPreviousPosition =  this.transform.position;
		Vector3 middlePreviousPosition =  baseMiddle.transform.position;

		applyMovementForce();

		Vector3 headCurrentPosition =  this.transform.position;
		Vector3 middleCurrentPosition =  baseMiddle.transform.position;
		followPreviousSegment(headPreviousPosition, headCurrentPosition, baseMiddle);
		followPreviousSegment(middlePreviousPosition, middleCurrentPosition, tail);

		// Inform animator of snake direction to trigger appropriate wiggle
		anim.SetFloat("SnakeVelocityX", rb2d.velocity.x);
		anim.SetFloat("SnakeVelocityY", rb2d.velocity.y);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.collider.name != "Cube") return;
		EventManager.HandleSnakeDeath();
		StartCoroutine(waitAndTriggerRespawn());
	}

	private void applyMovementForce() {
		Vector2 force = Vector2.zero;
		int rotation = 0;

		if (isInputDown() && rb2d.IsTravellingHorizontally()) {
			force = new Vector2(0, -1 * speed * Time.fixedDeltaTime);	
			rotation = 0;
		} else if (isInputUp() && rb2d.IsTravellingHorizontally()) {
			force = new Vector2(0, 1 * speed * Time.fixedDeltaTime);
			rotation = 180;
		} else if (isInputLeft() && rb2d.IsTravellingVertically()) {
			force = new Vector2(-1 * speed * Time.fixedDeltaTime, 0);
			rotation = 270;
		} else if (isInputRight() && rb2d.IsTravellingVertically()) {
			force = new Vector2(1 * speed * Time.fixedDeltaTime, 0);
			rotation = 90;
		}

		// If no input was entered this frame, the previously applied force will continue
		// to operate unimpeded, as we have disabled drag/gravity.
		if (force == Vector2.zero) return; 

		rb2d.velocity = Vector2.zero;
		rb2d.AddForce(force);
		rb2d.MoveRotation(rotation);
	}

	private IEnumerator waitAndTriggerRespawn() {
		//hide
		yield return new WaitForSeconds(0.3f);
		//show
		rb2d.transform.position = Vector3.zero; // Reset position to the center of the game
		rb2d.MoveRotation(0); // Reset head to face down
		speed = defaultSpeed;	
		rb2d.AddForce(new Vector2(0, -1 * speed * Time.fixedDeltaTime));
		yield return null;
	}

	private void speedUp() {
		speed += defaultSpeed / 10f;
	}

	private bool isInputDown() {
		return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
	}

	private bool isInputUp() {
		return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
	}

	private bool isInputRight() {
		return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
	}

	private bool isInputLeft() {
		return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
	}

	private void followPreviousSegment(Vector3 leadingObjPreviousPosition, Vector3 leadingObjCurrentPosition, GameObject trailingObj) {
		float moveX = leadingObjPreviousPosition.x - leadingObjCurrentPosition.x;
		float moveY = leadingObjPreviousPosition.y - leadingObjCurrentPosition.y;
		trailingObj.transform.Translate(moveX, moveY, 0); 
	}
}


