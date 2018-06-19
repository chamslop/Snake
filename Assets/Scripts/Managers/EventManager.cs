namespace Managers {

  // EventManager is the static class to which event delegates are bound and delegate handlers are called.
  // A GameObject can register a delegate to be handled inside Start/constructor and should 
  // deregister any delegates inside OnDestroy/deconstructor to prevent memory leaks.
  public static class EventManager {
    // SnakeDeath
    // WHEN:
    // - Snake collides with a wall
    // - Snake collides with itself
    public delegate void SnakeDeath();
    public static event SnakeDeath OnSnakeDeath;

    public static void HandleSnakeDeath() {
      if (OnSnakeDeath == null) return;
      OnSnakeDeath();
    }

    // PointScore
    // WHEN:
    // - Snake collides with a piece of food
    public delegate void PointScore();
    public static event PointScore OnPointScored;

    public static void HandlePointScored() {
      if (OnPointScored == null) return;
      OnPointScored();
    }
  }

}

