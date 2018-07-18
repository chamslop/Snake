using UnityEngine;

public class Food : MonoBehaviour {

	BoxCollider2D _col;

	void Awake() {
		_col = GetComponent<BoxCollider2D>();

		Managers.EventManager.OnSnakeMove += checkCollision;
		Managers.EventManager.OnSnakeDeath += repositionAwayFromSnake;
	}	

	void Start() {
		repositionAwayFromSnake();
	}

	void Destroy() {
		Managers.EventManager.OnSnakeMove -= checkCollision;
		Managers.EventManager.OnSnakeDeath -= repositionAwayFromSnake;
	}

	void checkCollision(Bounds b) {
		if (_col.bounds.Intersects(b)) {
			Managers.EventManager.BroadcastPointScored();
			repositionAwayFromSnake();
		}
	}

	private void repositionAwayFromSnake() {
		while(true) {
			Vector3 newPosition = Vector3Extensions.Random(-5, 5);
			if (Vector2.Distance(newPosition, transform.position) < 5.5f) continue;
			if (((uint) newPosition.x + (uint) transform.position.x) < -4.5 || 
				((uint) newPosition.x + (uint) transform.position.x) > 4.5 || 
				((uint) newPosition.y + (uint) transform.position.y) < -4.5 || 
				((uint) newPosition.y + (uint) transform.position.y) > 4.5) {
					continue;
			}

			transform.position = newPosition;
			return;
		}
	}
}
