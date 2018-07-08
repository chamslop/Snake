using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour {

	private int _score;
	private Text _text;

	void Awake() {
		_text = GetComponent<Text>();

		Managers.EventManager.OnPointScored += pointScored;
		Managers.EventManager.OnSnakeDeath += resetScore;
	}

	void Start() {
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
