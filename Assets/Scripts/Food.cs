using UnityEngine;

public class Food : MonoBehaviour {

	void Start() {
		repositionAwayFromSnake();
	}

	void OnTriggerEnter2D(Collider2D col) {
		Managers.EventManager.HandlePointScored();
		repositionAwayFromSnake();
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
