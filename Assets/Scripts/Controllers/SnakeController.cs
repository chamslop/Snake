using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Managers;

public class SnakeController : MonoBehaviour {
	// Components
	private BoxCollider2D _headCollider;
	private Transform _spriteHead;
	private GameObject _tail;

	// Internals
	private float _assetSize = 1.28f;
	private float _xDir;
	private float _yDir;
	private KeyCode _lastKeyPressed;
	private float _speed = 0.2f; // Time delay between snakeMove calls

	// Sprites
	private Sprite _bendSprite;
	private Sprite _straightBodySprite;
	private Sprite _tailSprite;

	// Consts
	private string _bendSpriteName = "char_corner_01";
	private string _straightBodySpriteName = "char_mid_01";
	private string _tailSpriteName = "char_tail_01";

	void Awake() {
		_headCollider = GetComponent<BoxCollider2D>();
		_spriteHead = this.transform.Find("Head Sprite");
		_tail = this.transform.parent.transform.Find("Tail").gameObject;

		_bendSprite = Resources.Load<Sprite>(_bendSpriteName);
		_straightBodySprite = Resources.Load<Sprite>(_straightBodySpriteName);
		_tailSprite = Resources.Load<Sprite>(_tailSpriteName);
	}

	void Start() {
		EventManager.OnPointScored += addBodySegment;
		EventManager.OnPointScored += speedUp;
		EventManager.OnSnakeDeath += resetSnake;

		startSnakeMovement();
	}

	void Update() {
		// Every frame, check for inputs so we know which the last triggered key press was
		KeyCode inputThisFrame = InputExtensions.GetLastDirectionalKeyPress();
		_lastKeyPressed = inputThisFrame != KeyCode.None ? inputThisFrame : _lastKeyPressed;
	}

	// snakeMove is called recursively, at the interval of 'speed', and is responsible for moving the head,
	// and then getting all of the body components and tail to follow in the typicall snake fashion.
	private IEnumerator snakeMove() {
		yield return new WaitForSeconds(_speed);

		// If our last key press was valid (i.e not a 180deg turn), prepare our x and y for translation
		// We also want to rotate the head at this point 
		if (_lastKeyPressed == KeyCode.UpArrow && _yDir == 0) {
			_spriteHead.rotation = new Quaternion(0, 0, 180, 0);
			_yDir = _assetSize;
			_xDir = 0;
		} else if (_lastKeyPressed == KeyCode.DownArrow && _yDir == 0) {
			_spriteHead.rotation = new Quaternion(0, 0, 0, 0);
			_yDir = -_assetSize;
			_xDir = 0;
		} else if (_lastKeyPressed == KeyCode.RightArrow && _xDir == 0) {
			_spriteHead.Rotate(0, 0, (int) _yDir * -90f, Space.Self);
			_xDir = _assetSize;
			_yDir = 0;
		} else if (_lastKeyPressed == KeyCode.LeftArrow && _xDir == 0) {
			_spriteHead.Rotate(0, 0, (int) _yDir * 90f, Space.Self);
			_xDir = -_assetSize;
			_yDir = 0;
		} else {
			// No valid key put was detected this update cycle, use previously assigned x and y
		}

		// Keep a record of where the head was/it's rotate before this translate
		Vector3 leadingSegmentPreviousPosition = this.transform.position;
		Quaternion leadingSegmentPreviousRotation = _spriteHead.transform.rotation;

		// Translate the head according to x and y
		this.transform.Translate(_xDir, _yDir, 0);

		// Before we move any body segments, first check for head => segment collisions
		if (checkHeadCollision()) {
			EventManager.HandleSnakeDeath();
		} else {
			// Player hasn't died, let's update body segments accordingly
			for (var i = 1; i < this.transform.parent.childCount; i++) {
				Transform currentSegment = this.transform.parent.GetChild(i);
				Vector3 currentSegmentPosition = currentSegment.position;
				SpriteRenderer currentSegmentSpriteRenderer = currentSegment.GetComponent<SpriteRenderer>();
				Transform leadingSegment = this.transform.parent.GetChild(i-1);

				// Check if currentSegment is a bend that should be straightened
				if (currentSegmentSpriteRenderer.sprite.name == _bendSpriteName && 
					!shouldRotate(currentSegment, leadingSegment, leadingSegmentPreviousPosition)) {
					currentSegmentSpriteRenderer.sprite = _straightBodySprite;
				}

				// Before moving our segment, see if we should rotate
				if (shouldRotate(currentSegment, leadingSegment, leadingSegmentPreviousPosition)) {
					currentSegment.rotation = leadingSegmentPreviousRotation;

					if (currentSegmentSpriteRenderer.sprite.name != _tailSpriteName) {
						currentSegmentSpriteRenderer.sprite = _bendSprite;
					}
				}

				// Update our current segment to the position of it's leading segment before it was transformed
				currentSegment.transform.position = leadingSegmentPreviousPosition;

				// Save the record of our current segments previous position/rotation for it's trailing segment to use 
				leadingSegmentPreviousPosition = currentSegmentPosition;
				leadingSegmentPreviousRotation = currentSegment.rotation;
			}

			// Inform the animator of our current direction
			// _anim.SetFloat("SnakeVelocityX", _xDir);
			// _anim.SetFloat("SnakeVelocityY", _yDir);

			// Broadcast that we have successfully moved the snake
			Managers.EventManager.HandleSnakeMove(_headCollider.bounds);

			// Recursively call snakeMove again so we continually update
			StartCoroutine(snakeMove());
		}

		yield return null;
	}

	private void resetSnake() {
		StartCoroutine(waitAndTriggerRespawn());
		Destroy(this.transform.parent.gameObject);
	}

	private IEnumerator waitAndTriggerRespawn() {
		yield return new WaitForSeconds(1f);
		yield return null;
	}

	private void addBodySegment() {
		Transform body = this.transform.parent.GetChild(this.transform.parent.childCount - 1);
		GameObject newBodySegment = Instantiate<GameObject>(body.gameObject);
		newBodySegment.GetComponent<SpriteRenderer>().sprite = _straightBodySprite;
		newBodySegment.transform.parent = body.parent;

		_tail.transform.SetAsLastSibling();
	}

	private void speedUp() {
		_speed -= (_speed/30);
	}

	private void startSnakeMovement() {
		_yDir = -_assetSize;
		_xDir = 0;
		StartCoroutine(snakeMove());
	}

	// checkHeadCollision checks if the player has 'died' by running the head into a body component,
	// achieved by checking for intersecting collider bounds.
	private bool checkHeadCollision() {
		for (var i = 1; i < this.transform.parent.childCount; i++) { 
			BoxCollider2D segmentCollider = this.transform.parent.GetChild(i).GetComponent<BoxCollider2D>();
			if (_headCollider.bounds.Intersects(segmentCollider.bounds)) {
				return true;
			}
		}

		return false;
	}

	// shouldRotate determines whether or not the current segment should be rotated BEFORE it has teleported.
	// We need to rotate before because we need to compare if the leading segment moved in the same direction
	// we are going to move in. If not, that means a change of direction occured, and we need to rotate accordingly.
	private bool shouldRotate(Transform currentSegment, Transform leadingSegment, Vector3 leadingSegmentPrev) {
		float teleportXDif = leadingSegmentPrev.x - currentSegment.position.x;
		float teleportYDif = leadingSegmentPrev.y - currentSegment.position.y;
		float leadingSegmentXDif = leadingSegment.position.x - leadingSegmentPrev.x;
		float leadingSegmentYDif = leadingSegment.position.y - leadingSegmentPrev.y;

		return teleportXDif != leadingSegmentXDif && teleportYDif != leadingSegmentYDif;
	}

	private Vector3 directionOfTravel(Vector3 current, Vector3 target) {
		return target - current;
	}

	private Vector3 previousDirection(Vector3 current, Vector3 previous) {
		return previous - current;
	}

}
