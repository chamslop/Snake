using UnityEngine;

public static class InputExtensions {
	
  public static bool IsInputDown()  => Input.GetKeyDown(KeyCode.DownArrow)  || Input.GetKeyDown(KeyCode.S);
	public static bool IsInputUp()    => Input.GetKeyDown(KeyCode.UpArrow)    || Input.GetKeyDown(KeyCode.W);
	public static bool IsInputRight() => Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
	public static bool IsInputLeft()  => Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.A);
	
  public static KeyCode GetLastDirectionalKeyPress() {
		if (IsInputUp()) {
			return KeyCode.UpArrow;
		} else if (IsInputDown()) {
			return KeyCode.DownArrow;
		} else if (IsInputRight()) {
			return KeyCode.RightArrow;
		} else if (IsInputLeft()) {
			return KeyCode.LeftArrow;
		}

    return KeyCode.None;
	}
}