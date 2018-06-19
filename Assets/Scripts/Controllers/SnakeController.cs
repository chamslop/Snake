using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Managers;

public class SnakeController : MonoBehaviour {
	// Components
	private Animator anim;
	private Rigidbody2D rb2d;

	// Internals
	private float speed = 100F;

	void Start() {
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();

		EventManager.OnPointScored += speedUp;

		rb2d.AddForce(new Vector2(0, 1 * speed));
	}
	
	void FixedUpdate() {
		applyMovementForce();
		anim.SetFloat("SnakeVelocityX", rb2d.velocity.x);
		anim.SetFloat("SnakeVelocityY", rb2d.velocity.y);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.collider.name != "Cube") return;
		Managers.EventManager.HandleSnakeDeath();
		StartCoroutine(waitAndTriggerRespawn());
	}

	private void applyMovementForce() {
		if (isInputDown() && isTravellingHorizontally()) {
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(0, -1 * speed));
			return;
		} else if (isInputUp() && isTravellingHorizontally()) {
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(0, 1 * speed));
			return;
		} else if (isInputLeft() && isTravellingVertically()) {
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(-1 * speed, 0));
			return;
		} else if (isInputRight() && isTravellingVertically()) {
			rb2d.velocity = Vector2.zero;
			rb2d.AddForce(new Vector2(1 * speed, 0));
		}
	}

	private IEnumerator waitAndTriggerRespawn() {
		//hide
		yield return new WaitForSeconds(0.3f);
		//show
		rb2d.transform.position = Vector3.zero;
		speed = 100f;
		rb2d.AddForce(new Vector2(0, 1 * speed));
		yield return null;
	}

	private void speedUp() {
		speed += 10f;
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

	private bool isTravellingHorizontally() {
		return rb2d.velocity.x != 0;
	}

	private bool isTravellingVertically() {
		return rb2d.velocity.y != 0;
	}
}


