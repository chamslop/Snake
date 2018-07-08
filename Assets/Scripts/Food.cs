using UnityEngine;

public class Food : MonoBehaviour {

	BoxCollider2D _col;

	void Awake() {
		_col = GetComponent<BoxCollider2D>();
	}	

	void Start() {
		repositionAwayFromSnake();

		Managers.EventManager.OnSnakeMove += checkCollision;
		Managers.EventManager.OnSnakeDeath += repositionAwayFromSnake;
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
			if (Vector2.Distance(newPosition, this.transform.position) < 5.5f) continue;
			this.transform.position = newPosition;
			return;
		}
	}
}
