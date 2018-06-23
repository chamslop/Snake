using System;
using UnityEngine;

public static class RigidBody2DExtensions {

  public static bool IsTravellingHorizontally(this Rigidbody2D rb2d) {
    return rb2d.velocity.x != 0;
  }

  public static bool IsTravellingVertically(this Rigidbody2D rb2d) {
    return rb2d.velocity.y != 0;
  }
}


