using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HeadVisible is a dedicated script soley used to broadcast when the head sprite leaves the bounds of the
// main camera. This needs to be separate from the Snek prefab because we are only asking if the head 
// is invisible, not the entirety of the snake.
public class HeadVisible : MonoBehaviour {

	void OnBecameInvisible() => EventManager.BroadcastSnakeHeadInvisible();

}
