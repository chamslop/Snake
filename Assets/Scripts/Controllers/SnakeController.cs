using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Managers;

public class SnakeController : MonoBehaviour {
	// Components
	private Animator anim;
	private BoxCollider2D headCollider;
	private GameObject tail;
	private SpriteRenderer spriteRenderer;

	// Internals
	private float assetSize = 1.28f;
	private float xDir;
	private float yDir;
	private KeyCode lastKeyPressed;

	void Start() {
		anim = GetComponent<Animator>();
		headCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		tail = this.transform.parent.transform.Find("Tail").gameObject;

		EventManager.OnPointScored += addBodySegment;

		startSnakeMovement();
	}

	void Update() {
		// Every frame, check for inputs so we know which the last triggered key press was
		if (isInputUp()) {
			lastKeyPressed = KeyCode.UpArrow;
		} else if (isInputDown()) {
			lastKeyPressed = KeyCode.DownArrow;
		} else if (isInputRight()) {
			lastKeyPressed = KeyCode.RightArrow;
		} else if (isInputLeft()) {
			lastKeyPressed = KeyCode.LeftArrow;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		EventManager.HandleSnakeDeath();
		StartCoroutine(waitAndTriggerRespawn());
	}

	private IEnumerator snakeMove() {
		yield return new WaitForSeconds(0.2f);

		// If our last key press was valid (i.e not a 180deg turn), prepare our x and y from translate 
		if (lastKeyPressed == KeyCode.UpArrow && yDir == 0) {
			yDir = assetSize;
			xDir = 0;
		} else if (lastKeyPressed == KeyCode.DownArrow && yDir == 0) {
			yDir = -assetSize;
			xDir = 0;
		} else if (lastKeyPressed == KeyCode.RightArrow && xDir == 0) {
			xDir = assetSize;
			yDir = 0;
		} else if (lastKeyPressed == KeyCode.LeftArrow && xDir == 0) {
			xDir = -assetSize;
			yDir = 0;
		} else {
			// No valid key put was detected this update cycle, use previously assigned x and y
		}

		// Keep a record of where the head was before this translate
		Vector3 leadingSegmentPreviousPosition = this.transform.position;

		// Translate the head according to x and y
		this.transform.Translate(xDir, yDir, 0);

		if (yDir == assetSize) {
			spriteRenderer.flipY = true;
		} else if (yDir == -assetSize) {
			spriteRenderer.flipY = false;
		}

		// Before we move any body segments, first check for head=>segment collisions
		if (headHasCollidedWithSegment()) {
			EventManager.HandleSnakeDeath();
			yield return null;
		}

		// Player hasn't died, let's update body segments accordingly
		for (var i = 1; i < this.transform.parent.childCount; i++) {
			Transform currentSegment = this.transform.parent.GetChild(i);
			Vector3 currentSegmentPosition = currentSegment.position;
			
			// Update our current segment to the position of it's leading segment before it was transformed
			currentSegment.transform.position = leadingSegmentPreviousPosition;

			// Save the record of our current segments previous position for it's trailing segment 
			leadingSegmentPreviousPosition = currentSegmentPosition;
		}

		// Inform the animator of our current direction
		anim.SetFloat("SnakeVelocityX", xDir);
		anim.SetFloat("SnakeVelocityY", yDir);

		// Broadcast that we have successfully moved the snake
		Managers.EventManager.HandleSnakeMove(headCollider.bounds);

		// Recursively call snakeMove again so we continually update
		StartCoroutine(snakeMove());

		yield return null;
	}

	private IEnumerator waitAndTriggerRespawn() {
		//hide
		yield return new WaitForSeconds(0.3f);
		//show

		startSnakeMovement();

		yield return null;
	}

	private void addBodySegment() {
		Transform body = this.transform.parent.GetChild(1);
		GameObject newBodySegment = Instantiate<GameObject>(body.gameObject);
		newBodySegment.transform.parent = body.parent;
		tail.transform.SetAsLastSibling();
	}

	private void startSnakeMovement() {
		yDir = -assetSize;
		xDir = 0;
		StartCoroutine(snakeMove());
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

	private bool headHasCollidedWithSegment() {
		for (var i = 1; i < this.transform.parent.childCount; i++) { 
			BoxCollider2D segmentCollider = this.transform.parent.GetChild(i).GetComponent<BoxCollider2D>();
			if (headCollider.bounds.Intersects(segmentCollider.bounds)) {
				return true;
			}
		}

		return false;
	}
}


