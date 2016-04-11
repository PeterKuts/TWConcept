using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public enum PointerPhase {
	Began,
	Moved,
	Ended,
	Canceled
}

public class Pointer {
	public readonly int id;
	public readonly Vector3 position;
	public readonly PointerPhase phase;
	public Pointer(int id, Vector3 position, PointerPhase phase) {
		this.id = id;
		this.position = position;
		this.phase = phase;
	}

	public static Pointer Canceled(int id) {
		return new Pointer (id, Vector3.zero, PointerPhase.Canceled);
	}
}

public class PlayerInput : ObservableTriggerBase {

	public IObservable<IObservable<Pointer>> Pointers {get {return FuncExt.CacheProperty(ref pointerSubjs);}}

	private Subject<IObservable<Pointer>> pointerSubjs;
	private Dictionary<int, Subject<Pointer>> activePointerSubjs = new Dictionary<int, Subject<Pointer>>();
	private Func<IEnumerable<Pointer>> GetPointers;

	void Start() {
		if (Input.touchSupported) {
			GetPointers = PointersFromTouches;
		} else if (Input.mousePresent) {
			GetPointers = PointersFromMouse;
		} else {
			GetPointers = Enumerable.Empty<Pointer>;
		}
	}

	void Update() {
		if (pointerSubjs == null) {
			return;
		}
		foreach (var p in GetPointers()) {
			OnPointer (p);
		}
	}

	void OnPointer(Pointer p) {
		switch (p.phase) {
		case PointerPhase.Began:
			OnPointerBegan (p);
			return;
		case PointerPhase.Moved:
			OnPointerMoved (p);
			return;
		case PointerPhase.Canceled:
		case PointerPhase.Ended:
			OnPointerEndedOrCanceled (p);
			return;
		}
	}

	void OnPointerBegan(Pointer p) {
		Subject<Pointer> subj;
		if (activePointerSubjs.TryGetValue (p.id, out subj)) {
			subj.OnNext (Pointer.Canceled(p.id));
			subj.OnCompleted ();
		}
		subj = new Subject<Pointer> ();
		activePointerSubjs [p.id] = subj;
		pointerSubjs.OnNext (subj);
		subj.OnNext (p);
	}

	void OnPointerMoved(Pointer p) {
		Subject<Pointer> subj;
		if (!activePointerSubjs.TryGetValue (p.id, out subj)) {
			return;
		}
		subj.OnNext (p);
	}

	void OnPointerEndedOrCanceled(Pointer p) {
		Subject<Pointer> subj;
		if (!activePointerSubjs.TryGetValue (p.id, out subj)) {
			return;
		}
		subj.OnNext (p);
		subj.OnCompleted ();
		activePointerSubjs.Remove (p.id);
	}

	static PointerPhase ToPointerPhase(TouchPhase phase) {
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

	static IEnumerable<Pointer> PointersFromTouches() {
		foreach (var t in Input.touches) {
			yield return new Pointer (t.fingerId, t.position, ToPointerPhase(t.phase));
		}
	}

	enum MouseButtons {
		Start = 0,
		Left = Start,
		Right,
		Middle,
		Count
	}

	static IEnumerable<Pointer> PointersFromMouse() {
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

	void CancelAllPointers() {
		foreach (var p in activePointerSubjs) {
			p.Value.OnNext (Pointer.Canceled(p.Key));
			p.Value.OnCompleted ();
		}
		activePointerSubjs.Clear ();
	}

	void OnApplicationFocus(bool focus) {
		if (!focus) {
			CancelAllPointers ();
		}
	}

	void OnDisable() {
		CancelAllPointers ();
	}

	protected override void RaiseOnCompletedOnDestroy()
	{
		CancelAllPointers ();
		if (pointerSubjs != null) {
			pointerSubjs.OnCompleted ();
			pointerSubjs = null;
		}
	}
}
