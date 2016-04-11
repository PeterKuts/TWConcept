using UnityEngine;
using System.Collections.Generic;

public static class InputExt  {

	public static IEnumerable<Pointer> TouchesPointers() {
		foreach (var t in Input.touches) {
			yield return new Pointer (t.fingerId, t.phase.ToInputPhase(), t.position);
		}
	}

	public static IEnumerable<Pointer> MousePointers() {
		for (int mouseButton = (int)MouseButtons.Start; mouseButton < (int)MouseButtons.Count; ++mouseButton) {
			if (Input.GetMouseButtonDown (mouseButton)) {
				yield return new Pointer (mouseButton, InputPhase.Began, Input.mousePosition);
			}
			if (Input.GetMouseButton (mouseButton)) {
				yield return new Pointer (mouseButton, InputPhase.Hold, Input.mousePosition);
			}
			if (Input.GetMouseButtonUp (mouseButton)) {
				yield return new Pointer (mouseButton, InputPhase.Ended, Input.mousePosition);
			}
		}
	}

	public static IEnumerable<Key> KeysFromCodes(IEnumerable<KeyCode> keyCodes) {
		foreach (var c in keyCodes) {
			if (Input.GetKeyDown (c)) {
				yield return new Key (c, InputPhase.Began);
			}
			if (Input.GetKey (c)) {
				yield return new Key (c, InputPhase.Hold);
			}
			if (Input.GetKeyUp (c)) {
				yield return new Key (c, InputPhase.Ended);
			}
		}
	}

	enum MouseButtons {
		Start = 0,
		Left = Start,
		Right,
		Middle,
		Count
	}

	static InputPhase ToInputPhase(this TouchPhase phase) {
		switch (phase) {
		case TouchPhase.Began:
			return InputPhase.Began;
		case TouchPhase.Moved:
		case TouchPhase.Stationary:
			return InputPhase.Hold;
		case TouchPhase.Ended:
			return InputPhase.Ended;
		case TouchPhase.Canceled:
		default:
			return InputPhase.Canceled;
		}
	}

}
