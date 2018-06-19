using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Managers.EventManager.HandlePointScored();
		System.Random rnd = new System.Random();
		int x = rnd.Next(-5, 5);
		int y = rnd.Next(-5, 5);
		this.transform.position = new Vector3(x, y, 0);
	}

}
