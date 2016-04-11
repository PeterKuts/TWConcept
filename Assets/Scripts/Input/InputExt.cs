using UnityEngine;
using System.Collections.Generic;

public static class InputExt  {

	public static IEnumerable<Pointer> TouchesPointers() {
		foreach (var t in Input.touches) {
			yield return new Pointer (t.fingerId, t.position, t.phase.ToPointerPhase());
		}
	}

	public static IEnumerable<Pointer> MousePointers() {
		for (int mouseButton = (int)MouseButtons.Start; mouseButton < (int)MouseButtons.Count; ++mouseButton) {
			if (Input.GetMouseButtonDown (mouseButton)) {
				yield return new Pointer (mouseButton, Input.mousePosition, PointerPhase.Began);
			}
			if (Input.GetMouseButton (mouseButton)) {
				yield return new Pointer (mouseButton, Input.mousePosition, PointerPhase.Moved);
			}
			if (Input.GetMouseButtonUp (mouseButton)) {
				yield return new Pointer (mouseButton, Input.mousePosition, PointerPhase.Ended);
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

	static PointerPhase ToPointerPhase(this TouchPhase phase) {
		switch (phase) {
		case TouchPhase.Began:
			return PointerPhase.Began;
		case TouchPhase.Moved:
		case TouchPhase.Stationary:
			return PointerPhase.Moved;
		case TouchPhase.Ended:
			return PointerPhase.Ended;
		case TouchPhase.Canceled:
		default:
			return PointerPhase.Canceled;
		}
	}

}
