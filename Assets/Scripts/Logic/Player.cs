using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;

public class Player : MonoBehaviour {

	private IPointerInput pointerInput { get { return Holder.SharedHolder.PointerInput;}}
	private IKeyInput keyInput { get { return Holder.SharedHolder.KeyInput;}}

	[SerializeField]
	private Rigidbody2D body;
	[SerializeField]
	private Rigidbody2D hookPrefab;

	private Dictionary <int, Rigidbody2D> activeHooks = new Dictionary<int, Rigidbody2D>();

	private Transform _transform;
	public Transform Transform {get {return this.CacheComponent (ref _transform);}}

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

	void PointerBegan(Pointer p) {
		var pos = (Vector2)Camera.main.ScreenToWorldPoint (p.position);
		var hook = (Rigidbody2D)GameObject.Instantiate (hookPrefab, body.position, Quaternion.identity);
		hook.transform.SetParent (Transform);
		hook.velocity = (pos - body.position).normalized * 10.0f;
		hook.OnTriggerEnter2DAsObservable ().Subscribe (c => {
			if (c == body.GetComponent<Collider2D>()) {
				Debug.Log("Skip");
			} else {
				hook.velocity = Vector2.zero;
				Debug.Log("Collide");
			}
		});
		activeHooks [p.key] = hook;
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
