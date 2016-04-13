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

	private CompositeDisposable disposables = new CompositeDisposable();

	void Start () {
		pointerInput.PointersObservable.Subscribe (CreateHook).AddTo(disposables);
	}

	void CreateHook(IObservable<Pointer> pointers) {
		var hook = (Hook)GameObject.Instantiate (hookPrefab, body.Transform.position, Quaternion.identity);
		hook.Transform.SetParent (Transform);
		hook.Init (pointers, this.FixedUpdateAsObservable ());
	}

	void OnDestroy() {
		disposables.Dispose ();
	}

}
