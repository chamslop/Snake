using UnityEngine;

public class Food : MonoBehaviour {

	BoxCollider2D col;

	void Start() {
		col = GetComponent<BoxCollider2D>();
		repositionAwayFromSnake();

		Managers.EventManager.OnSnakeMove += checkCollision;
	}

	void OnDestroy() {
		Managers.EventManager.OnSnakeMove -= checkCollision;
	}

	void checkCollision(Bounds b) {
		if (col.bounds.Intersects(b)) {
			Managers.EventManager.HandlePointScored();
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
