using UnityEngine;

namespace Managers {

  // EventManager is the static class to which event delegates are bound and delegate handlers are called.
  // A GameObject can register a delegate to be handled inside Awake/constructor and should 
  // deregister any delegates inside OnDestroy/deconstructor to prevent memory leaks.
  public static class EventManager {

    public delegate void SnakeMove(Bounds b);
    public static event SnakeMove OnSnakeMove;
    public static void BroadcastSnakeMove(Bounds b) => OnSnakeMove?.Invoke(b);

    public delegate void SnakeHeadInvisible();
    public static event SnakeHeadInvisible OnSnakeHeadInvisible;
    public static void BroadcastSnakeHeadInvisible() => OnSnakeHeadInvisible?.Invoke();

    public delegate void SnakeDeath();
    public static event SnakeDeath OnSnakeDeath;
    public static void BroadcastSnakeDeath() => OnSnakeDeath?.Invoke();

    public delegate void PointScore();
    public static event PointScore OnPointScored;
    public static void BroadcastPointScored() => OnPointScored?.Invoke();
    
  }

}

