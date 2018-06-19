using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour {

	private Text text;
	private int score;

	void Start() {	
		text = GetComponent<Text>();
		
		Managers.EventManager.OnPointScored += pointScored;
		Managers.EventManager.OnSnakeDeath += resetScore;

		score = 0;
		text.text = "Score: 0";
	}

	void OnDestroy() {
		Managers.EventManager.OnPointScored -= pointScored;
	}

	private void pointScored() {
		score++;
		text.text = "Score: " + score;
	}

	private void resetScore() {
		score = 0;
		text.text = "Score: " + score;
	}
}
