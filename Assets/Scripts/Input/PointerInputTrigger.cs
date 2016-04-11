using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class Pointer: InputEntity<int> {
	public readonly Vector3 position;
	public Pointer(int key, InputPhase phase, Vector3 position): base(key, phase) {
		this.position = position;
	}

	public static Pointer Canceled(int key) {
		return new Pointer (key, InputPhase.Canceled, Vector3.zero);
	}
}

public interface IPointerInput {
	IObservable<IObservable<Pointer>> PointersObservable { get;}
}

public class PointerInputTrigger : ObservableTriggerBase, IPointerInput {

	private InputTrigger<int, Pointer> pointerTrigger;
	public IObservable<IObservable<Pointer>> PointersObservable  {get {return pointerTrigger.InputObservables;}}

	static Func<IEnumerable<Pointer>> PointerInjection() {
		if (Input.touchSupported) {
			return InputExt.TouchesPointers;
		} else if (Input.mousePresent) {
			return InputExt.MousePointers;
		}
		return null;
	}

	void Awake() {
		pointerTrigger = new InputTrigger<int, Pointer> (PointerInjection (), Pointer.Canceled);
	}

	void OnApplicationFocus(bool focus) {
		if (!focus) {
			pointerTrigger.CancelAllInput();
		}
	}

	void OnDisable() {
		pointerTrigger.CancelAllInput();
	}

	protected override void RaiseOnCompletedOnDestroy() {
		pointerTrigger.CancelDestroyAllInput();
	}

	void Update() {
		pointerTrigger.Update ();
	}

}
