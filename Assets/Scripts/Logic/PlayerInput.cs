using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public enum PointerPhase {
	Began,
	Moved,
	Ended
}

public struct Pointer {
	public readonly int id;
	public readonly Vector2 position;
	public readonly PointerPhase phase;
	public Pointer(int id, Vector2 position, PointerPhase phase) {
		this.id = id;
		this.position = position;
		this.phase = phase;
	}
}

public class PlayerInput : ObservableTriggerBase {

	private Subject<IObservable<Pointer>> pointers;
	public IObservable<IObservable<Pointer>> Pointers {get {return FuncExt.CacheProperty(ref pointers);}}

	//private Dictionary<int, Subject<Pointer>> activePointers = new Dictionary<int, Subject<Pointer>>();

	private Subject<Pointer> activePointer = null;
	void Update() {
		if (pointers == null) {
			return;
		}
		bool inputSkiped = true;
		if (Input.GetMouseButtonUp (0)) {
			inputSkiped = false;
			activePointer = new Subject<Pointer> ();
			pointers.OnNext (activePointer);
			activePointer.OnNext (new Pointer (0, Input.mousePosition, PointerPhase.Began));
		} 
		if (Input.GetMouseButton (0)) {
			inputSkiped = false;
			activePointer.OnNext (new Pointer (0, Input.mousePosition, PointerPhase.Moved));
		}
		if (Input.GetMouseButtonDown (0)) {
			inputSkiped = false;
			activePointer.OnNext (new Pointer (0, Input.mousePosition, PointerPhase.Ended));
			activePointer.OnCompleted ();
			activePointer = null;
		}
		if (inputSkiped) {
			
		}
	}

	void OnDisable() {
	}

	protected override void RaiseOnCompletedOnDestroy()
	{
		if (activePointer != null) {
			activePointer.OnCompleted ();
			activePointer = null;
		}
		if (pointers != null) {
			pointers.OnCompleted ();
			pointers = null;
		}
	}
}
