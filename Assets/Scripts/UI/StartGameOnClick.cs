using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameOnClick : MonoBehaviour {

	public void StartGame() => UIEventManager.BroadcastStartGame();

}
