using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public enum MouseState {
	Down,
	Hold,
	Up,
}

public struct MouseData {
	public readonly MouseState state;
	public readonly Vector3 position;
	public MouseData(MouseState state, Vector3 position) {
		this.state = state;
		this.position = position;
	}
}

public interface IInputTrigger {
	IObservable<float> HorizontalAxis { get;}
	IObservable<float> VerticalAxis { get;}
	IObservable<Unit> AKey { get;}
	IObservable<Unit> SKey { get;}
	IObservable<Unit> DKey { get;}
	IObservable<MouseData> Mouse { get;}
}

public class InputTrigger : ObservableTriggerBase, IInputTrigger {

	private Subject<float> horizontalAxis;
	public IObservable<float> HorizontalAxis { get { return FuncExt.CacheProperty (ref horizontalAxis);}}
	private Subject<float> verticalAxis;
	public IObservable<float> VerticalAxis { get { return FuncExt.CacheProperty (ref verticalAxis);}}
	private Subject<Unit> aKey;
	public IObservable<Unit> AKey { get { return FuncExt.CacheProperty (ref aKey);}}
	private Subject<Unit> sKey;
	public IObservable<Unit> SKey { get { return FuncExt.CacheProperty (ref sKey);}}
	private Subject<Unit> dKey;
	public IObservable<Unit> DKey { get { return FuncExt.CacheProperty (ref dKey);}}
	private Subject<MouseData> mouse;
	public IObservable<MouseData> Mouse { get { return FuncExt.CacheProperty (ref mouse);}}

	void Update() {
		FuncExt.CallNext (horizontalAxis, () => Input.GetAxis ("Horizontal"));
		FuncExt.CallNext (verticalAxis, () => Input.GetAxis ("Vertical"));
		FuncExt.CallNext (aKey, () => Input.GetKeyDown (KeyCode.A), () => Unit.Default);
		FuncExt.CallNext (sKey, () => Input.GetKeyDown (KeyCode.S), () => Unit.Default);
		FuncExt.CallNext (dKey, () => Input.GetKeyDown (KeyCode.D), () => Unit.Default);
		FuncExt.CallNext (mouse, () => Input.GetMouseButtonDown (0), () => new MouseData(MouseState.Down, Input.mousePosition));
		FuncExt.CallNext (mouse, () => Input.GetMouseButton (0), () => new MouseData(MouseState.Hold, Input.mousePosition));
		FuncExt.CallNext (mouse, () => Input.GetMouseButtonUp (0), () => new MouseData(MouseState.Up, Input.mousePosition));
	}

	protected override void RaiseOnCompletedOnDestroy()
	{
		FuncExt.CallComplete (horizontalAxis);
		FuncExt.CallComplete (verticalAxis);
		FuncExt.CallComplete (aKey);
		FuncExt.CallComplete (sKey);
		FuncExt.CallComplete (dKey);
		FuncExt.CallComplete (mouse);
	}
}
