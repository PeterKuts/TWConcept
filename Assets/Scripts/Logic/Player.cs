using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;

public class Player : MonoBehaviour {

	[SerializeField]
	private Rigidbody2D rigidBody;
	[SerializeField]
	private ObservableFixedUpdateTrigger fixedUpdateTrigger;

	void Start () {
		var inputTrigger = Holder.SharedHolder.InputTrigger;
		inputTrigger.Mouse
			.Where(_ => {return isActiveAndEnabled;})
			.Buffer(fixedUpdateTrigger.FixedUpdateAsObservable())
			.SelectMany(list => list)
			.Subscribe(md => ActionForMouseState(md.state)(md.position));
	}

	Action<Vector3> ActionForMouseState(MouseState st) {
		switch (st) {
		case MouseState.Down: 
			return MouseDown;
		case MouseState.Hold: 
			return MouseHold;
		case MouseState.Up: 
			return MouseUp;
		}
		return _ => {};
	}

	void MouseDataApply(MouseData md) {
		rigidBody.AddForce(((Vector2)Camera.main.ScreenToWorldPoint(md.position) - rigidBody.position).normalized * 2.0f, ForceMode2D.Impulse);
	}

	void MouseDown(Vector3 md) {
		Debug.Log ("Down");
		//GetComponent<SpriteRenderer> ().color = Color.green;
	}

	void MouseHold(Vector3 md) {
	}

	void MouseUp(Vector3 md) {
		Debug.Log ("Up");
		//GetComponent<SpriteRenderer> ().color = Color.white;
	}
}
