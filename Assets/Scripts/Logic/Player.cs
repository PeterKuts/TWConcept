using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;

public class Player : MonoBehaviour {

	[SerializeField]
	private PlayerInput playerInput;
	[SerializeField]
	private ObservableFixedUpdateTrigger fixedUpdateTrigger;

	void Start () {
		playerInput.Pointers
			.SelectMany(t => t
				.Buffer(fixedUpdateTrigger.FixedUpdateAsObservable())
				.SelectMany(l => l))
			.Subscribe(PoitnerOnNext);
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
		Debug.Log ("Began " + p.id);
	}

	void PointerMoved(Pointer p) {
	}

	void PointerEnded(Pointer p) {
		Debug.Log ("Ended " + p.id);
	}

	void PointerCanceled(Pointer p) {
		Debug.Log ("Canceled " + p.id);
	}
}
