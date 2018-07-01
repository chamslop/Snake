using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour {

	private Text _text;
	private int _score;

	void Awake() {
		_text = GetComponent<Text>();
	}

	void Start() {
		Managers.EventManager.OnPointScored += pointScored;
		Managers.EventManager.OnSnakeDeath += resetScore;

		resetScore();
	}

	void Destroy() {
		Managers.EventManager.OnPointScored -= pointScored;
		Managers.EventManager.OnSnakeDeath -= resetScore;
	}

	private void pointScored() {
		_score++;
		_text.text = "Score: " + _score;
	}

	private void resetScore() {
		_score = 0;
		_text.text = "Score: 0";
	}
}
