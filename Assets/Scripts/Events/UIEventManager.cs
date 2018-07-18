using UnityEngine;

namespace Managers {

  // UIEventManager is the static class to which UI event delegates are bound and delegate handlers are called.
  // A GameObject can register a delegate to be handled inside Awake/constructor and should 
  // deregister any delegates inside OnDestroy/deconstructor to prevent memory leaks.
  public static class UIEventManager {

    public delegate void StartGame();
    public static event StartGame OnStartGame;
    public static void BroadcastStartGame() => OnStartGame?.Invoke();

  }

}

