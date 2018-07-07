﻿using System.Collections;
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

	// Sprites
	private Sprite _bendSprite;
	private Sprite _bendInvertedSprite;
	private Sprite _straightBodySprite;
	private Sprite _tailSprite;

	// Consts
	private string _bendSpriteName         = "char_corner_01";
	private string _bendInvertedSpriteName = "char_corner_02";
	private string _straightBodySpriteName = "char_mid_01";
	private string _tailSpriteName         = "char_tail_01";

	// Internals
	private float _assetSize = 1.28f;
	private float _xDir;
	private float _yDir;
	private KeyCode _lastKeyPressed;
	private float _speed = 0.2f;          // Time delay between snakeMove calls
	private bool _addBodySegment = false; // Should we add a body part to the snake during next movement?

	void Awake() {
		_headCollider = GetComponent<BoxCollider2D>();
		_spriteHead = this.transform.Find("Head Sprite");
		_tail = this.transform.parent.transform.Find("Tail").gameObject;

		_bendSprite = Resources.Load<Sprite>(_bendSpriteName);
		_bendInvertedSprite = Resources.Load<Sprite>(_bendInvertedSpriteName);
		_straightBodySprite = Resources.Load<Sprite>(_straightBodySpriteName);
		_tailSprite = Resources.Load<Sprite>(_tailSpriteName);
	}

	void Start() {
		EventManager.OnPointScored += queueAddBodySegment;
		EventManager.OnPointScored += speedUp;
		EventManager.OnSnakeDeath += resetSnake;

		startSnakeMovement();
	}

	void Update() {
		// Every frame, check for inputs so we know which the last triggered key press was
		KeyCode inputThisFrame = InputExtensions.GetLastDirectionalKeyPress();
		_lastKeyPressed = inputThisFrame != KeyCode.None ? inputThisFrame : _lastKeyPressed;
	}

	// snakeMove is called recursively, at the interval of '_speed', and is responsible for moving the head,
	// and then getting all of the body components and tail to follow in the typical snake fashion.
	private IEnumerator snakeMove() {
		yield return new WaitForSeconds(_speed);

		// Update _xDir and _yDir for translation, as well as rotate our head according to last key pressed
		calculateHeadTranslation();

		// Keep a record of where the head was/it's rotate before this translate
		Vector3 leadingSegmentPreviousPosition = this.transform.position;
		Quaternion leadingSegmentPreviousRotation = _spriteHead.transform.rotation;
		Sprite leadingSegmentPreviousSprite = null;

		// Translate the head according to x and y
		this.transform.Translate(_xDir, _yDir, 0);
		
		// Before we move any body segments, first check for head => segment collisions
		if (checkHeadCollision()) {
			EventManager.HandleSnakeDeath();
		} else {
			// Player hasn't died, let's update body segments accordingly
			for (var i = 1; i < this.transform.parent.childCount; i++) {
				Transform currentSegment = this.transform.parent.GetChild(i);

				// Keep a reference to our current segments position, sprite and rotation before changes
				Vector3 currentSegmentPosition = currentSegment.position;
				Quaternion currentSegmentRotation = currentSegment.rotation;
				Sprite currentSegmentSprite = currentSegment.GetComponent<SpriteRenderer>().sprite;

				// Before we move anything, let's calculate rotations and sprites
				if (i == 1) {
					//The second segment is key, once it is set correctly, all subsequent pieces can just copy from it
					adjustSecondSegment(currentSegment, leadingSegmentPreviousPosition, leadingSegmentPreviousRotation);
				} else if (i < this.transform.parent.childCount - 1) {
					// This segment is somewhere behind the second segment. It can safely just copy both the
					// sprite and the rotation from it's leading segment before being moved
					currentSegment.rotation = leadingSegmentPreviousRotation;
					currentSegment.GetComponent<SpriteRenderer>().sprite = leadingSegmentPreviousSprite;
				} else {
					// We are at the tail. Either don't move (because a new segment is filling the space infront of us)
					// or move forward as expected (because the player didn't gain a point this movement phase)
					if (_addBodySegment) {
						addBodySegment();

						// Update current segment to be our newly added piece. We will update it's location to be where
						// we expected the tail to move to
						currentSegment = this.transform.parent.GetChild(i);

						// The newly added piece might need to be a bend
						currentSegment.GetComponent<SpriteRenderer>().sprite = leadingSegmentPreviousSprite;
					}

					currentSegment.rotation = leadingSegmentPreviousRotation;

					// Now, current segment is either the tail or the newly added body piece, update it's position
				}

				// Now we update our current segment to the position of it's leading segment before it was transformed
				currentSegment.transform.position = leadingSegmentPreviousPosition;

				// Save the record of our current segments previous position/rotation for it's trailing segment to use 
				leadingSegmentPreviousPosition = currentSegmentPosition;
				leadingSegmentPreviousRotation = currentSegmentRotation;
				leadingSegmentPreviousSprite = currentSegmentSprite;
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

	// calculateHeadTranslation checks  our last key press was valid (i.e not a 180deg turn), 
	// and calculates the x and y we need to apply for our head's translation this movement phase.
	// We update properties so that we can cross reference previously set values (firstly so we can check
	// for 'about turns', and secondly so if no input was registered, we can apply the same movement again).
	// We also want to rotate the head at this point.
	private void calculateHeadTranslation() {
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

	// adjustSecondSegment calculates the rotation and sprite for the second segment of the snake.
	// These values can all be calculated by using the current head position as well as the previous rotation/
	// position that the head was previously in. This function should be called AFTER the head has been translated,
	// but BEFORE the second segment has been translated. This means the position/rotation of the second segment 
	// at this point can be considered 'outdated', while the previous head position is the position we will be 
	// moving into after we have calculated our rotation/sprite.
	private void adjustSecondSegment(Transform cur, Vector3 previousHeadPosition, Quaternion previousHeadRotation) {
		SpriteRenderer currentSegmentSpriteRenderer = cur.GetComponent<SpriteRenderer>();
		Transform head = this.transform.parent.GetChild(0);

		bool isBend = isBendSprite(currentSegmentSpriteRenderer);
		bool rotate = shouldRotate(head, previousHeadPosition, cur);
		// Check if segment is a bend that should be straightened
		if (isBend && !rotate) {
			currentSegmentSpriteRenderer.sprite = _straightBodySprite;
		} else if (rotate) {
			// We are shouldn't be straight, that means we turned and need to swap sprites and rotate
			cur.rotation = previousHeadRotation;

			// If we are turning left, we need to use the inverted bend sprite
			if (isLeftHandTurn(head.position, previousHeadPosition, cur.position)) {
				currentSegmentSpriteRenderer.sprite = _bendInvertedSprite;
			} else {
				currentSegmentSpriteRenderer.sprite = _bendSprite;
			}
		}
	}

	// shouldRotate determines whether or not the current segment should be rotated BEFORE it has teleported.
	// We need to rotate before because we need to compare if the leading segment moved in the same direction
	// we are going to move in. If not, that means a change of direction occured, and we need to rotate accordingly.
	private bool shouldRotate(Transform head, Vector3 headPrevious, Transform cur) {
		// Next move dif represents the difference where the current segment will move to next movement phase
		float nextMoveXDif = headPrevious.x - cur.position.x;
		float nextMoveYDif = headPrevious.y - cur.position.y;

		// Head dif represents the difference the head travelled during it's last movement
		float headXDif = head.position.x - headPrevious.x;
		float headYDif = head.position.y - headPrevious.y;

		return nextMoveXDif != headXDif && nextMoveYDif != headYDif;
	}

	// These functions determine the direction this area of the snake was previous travelling by checking
	// the unupdated location of the trailing segment against the current. It also ensuring only 1 direction
	// of change is present, use the diagonal movement funcs to check for those patterns of movement.
	private bool wasTravellingDown(Vector3 headPrev, Vector3 cur) => cur.y > headPrev.y && cur.x == headPrev.x;
	private bool wasTravellingUp(Vector3 headPrev, Vector3 cur) => cur.y < headPrev.y && cur.x == headPrev.x;
	private bool wasTravellingRight(Vector3 headPrev, Vector3 cur) => cur.x < headPrev.x && cur.y == headPrev.y;
	private bool wasTravellingLeft(Vector3 headPrev, Vector3 cur) => cur.x > headPrev.x && cur.y == headPrev.y;

	private bool isTurningDown(Vector3 head, Vector3 headPrev) => head.y < headPrev.y && head.x == headPrev.x;
	private bool isTurningUp(Vector3 head, Vector3 headPrev) => head.y > headPrev.y && head.x == headPrev.x;
	private bool isTurningLeft(Vector3 head, Vector3 headPrev) => head.x < headPrev.x && head.y == headPrev.y;
	private bool isTurningRight(Vector3 head, Vector3 headPrev) => head.x > headPrev.x && head.y == headPrev.y;

	// isLeftHandTurn determines if the snake is turning left according to it's current direction (not the key
	// press!). For example, if the snake is travelling downwards, 'left' is actually 'right'. 
	private bool isLeftHandTurn(Vector3 head, Vector3 headPrev, Vector3 cur) {
		return (wasTravellingDown(headPrev, cur) && isTurningRight(head, headPrev)) || 
					 (wasTravellingRight(headPrev, cur) && isTurningUp(head, headPrev))   ||
					 (wasTravellingUp(headPrev, cur) && isTurningLeft(head, headPrev))    ||
					 (wasTravellingLeft(headPrev, cur) && isTurningDown(head, headPrev));
	}

	private bool isTailSprite(SpriteRenderer sr) => sr.sprite.name == _tailSpriteName;
	private bool isInvertedBendSprite(SpriteRenderer sr) => sr.sprite.name == _bendInvertedSpriteName;
	private bool isBendSprite(SpriteRenderer sr) => sr.sprite.name == _bendSpriteName || isInvertedBendSprite(sr);

	// queueAddBodySegment sets us up to add a body segment to the snake during the next movement phase.
	// We can't do it outside of movement or the additional piece will be janky.
	private void queueAddBodySegment() {
		_addBodySegment = true;
	}

	// addBodySegment actually adds the required body segment to the snake and should be called during movement
	// updates at the point just before the tail moves.
	private void addBodySegment() {
		Transform currentFinalBodySegment = this.transform.parent.GetChild(this.transform.parent.childCount - 1);
		GameObject newBodySegment = Instantiate<GameObject>(currentFinalBodySegment.gameObject);
		newBodySegment.GetComponent<SpriteRenderer>().sprite = _straightBodySprite;
		newBodySegment.transform.parent = currentFinalBodySegment.parent;

		_tail.transform.SetAsLastSibling();

		// Reset until next time a point is scored
		_addBodySegment = false;
	}

	private void resetSnake() {
		StartCoroutine(waitAndTriggerRespawn());
		Destroy(this.transform.parent.gameObject);
	}

	private IEnumerator waitAndTriggerRespawn() {
		yield return new WaitForSeconds(5f);
		yield return null;
	}
	
	// speedUp reduces the time between movement updates, giving the illusion the snake is speeding up.
	private void speedUp() => _speed -= (_speed/30);

	private void startSnakeMovement() {
		_yDir = -_assetSize;
		_xDir = 0;
		StartCoroutine(snakeMove());
	}
}

