using System;
using UnityEngine;

public static class Vector3Extensions {

  public static Vector3 Random(int min, int max) {
    System.Random rnd = new System.Random();
    return new Vector3(rnd.Next(min, max), rnd.Next(min, max), 0);
  }

}


