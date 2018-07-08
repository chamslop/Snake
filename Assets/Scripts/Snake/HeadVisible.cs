using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Managers;

public class HeadVisible : MonoBehaviour {

	void OnBecameInvisible() => EventManager.BroadcastSnakeHeadInvisible();

}
