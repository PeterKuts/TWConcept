using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;

public class Player : MonoBehaviour {

	[SerializeField]
	private PlayerInput playerInput;
	[SerializeField]
	private Rigidbody2D body;
	[SerializeField]
	private Rigidbody2D hookPrefab;

	private Dictionary <int, Rigidbody2D> activeHooks = new Dictionary<int, Rigidbody2D>();

	private Transform _transform;
	public Transform Transform {get {return this.CacheComponent (ref _transform);}}

	CompositeDisposable disposables = new CompositeDisposable();

	void Start () {
		playerInput.Pointers
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
		case PointerPhase.Began:
			PointerBegan(p);
			return;
		case PointerPhase.Moved:
			PointerMoved(p);
			return;
		case PointerPhase.Ended: 
			PointerEnded(p);
			return;
		case PointerPhase.Canceled: 
			PointerCanceled(p);
			return;
		}
	}

	void PointerBegan(Pointer p) {
		var pos = (Vector2)Camera.main.ScreenToWorldPoint (p.position);
		var hook = (Rigidbody2D)GameObject.Instantiate (hookPrefab, body.position, Quaternion.identity);
		hook.transform.SetParent (Transform);
		hook.velocity = (pos - body.position).normalized * 5.0f;
		activeHooks [p.id] = hook;
	}

	void PointerMoved(Pointer p) {
	}

	void PointerEnded(Pointer p) {
		GameObject.Destroy (activeHooks [p.id].gameObject);
		activeHooks.Remove (p.id);
	}

	void PointerCanceled(Pointer p) {
		GameObject.Destroy (activeHooks [p.id].gameObject);
		activeHooks.Remove (p.id);
	}
}
