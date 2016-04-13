using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;

public class Player: MonoBehaviour {

	private IPointerInput pointerInput { get { return Holder.SharedHolder.PointerInput;}}
	private IKeyInput keyInput { get { return Holder.SharedHolder.KeyInput;}}

	[SerializeField]
	private Transform _transform;
	public Transform Transform {get { return this.CacheComponent<Transform>(ref _transform); }}

	[SerializeField]
	private Body body;
	[SerializeField]
	private Hook hookPrefab;


	private Dictionary <int, Hook> activeHooks = new Dictionary<int, Hook>();

	CompositeDisposable disposables = new CompositeDisposable();

	void Start () {
		pointerInput.PointersObservable
			.SelectMany(t => t
				.Buffer(this.FixedUpdateAsObservable ())
				.SelectMany(l => l))
			.Subscribe(PoitnerOnNext).AddTo(disposables);
	}

	void OnDestroy() {
		disposables.Dispose ();
	}

	void PoitnerOnNext(Pointer p) {
		switch (p.phase) {
		case InputPhase.Began:
			PointerBegan(p);
			return;
		case InputPhase.Hold:
			PointerMoved(p);
			return;
		case InputPhase.Ended: 
			PointerEnded(p);
			return;
		case InputPhase.Canceled: 
			PointerCanceled(p);
			return;
		}
	}

	static Hook InstantiateHook(Hook prefab, Body body, Transform parent) {
		var hook = (Hook)GameObject.Instantiate (prefab, body.Transform.position, Quaternion.identity);
		Physics2D.IgnoreCollision (hook.Collider, body.Collider, true);
		hook.Transform.SetParent (parent);
		return hook;
	}

	void PointerBegan(Pointer p) {
		var pos = (Vector2)Camera.main.ScreenToWorldPoint (p.position);
		var hook = InstantiateHook(hookPrefab, body, Transform);
		activeHooks [p.key] = hook;
		hook.Rigidbody.velocity = (pos - body.Rigidbody.position).normalized * 100.0f;
	}

	void PointerMoved(Pointer p) {
	}

	void PointerEnded(Pointer p) {
		GameObject.Destroy (activeHooks [p.key].gameObject);
		activeHooks.Remove (p.key);
	}

	void PointerCanceled(Pointer p) {
		GameObject.Destroy (activeHooks [p.key].gameObject);
		activeHooks.Remove (p.key);
	}
}
