using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStartGame : MonoBehaviour {

	Canvas canvas;

	void Awake() {
		canvas = GetComponentInParent<Canvas>();
		UIEventManager.OnStartGame += Hide;
	}

	private void Hide() => canvas.enabled = false;

}
