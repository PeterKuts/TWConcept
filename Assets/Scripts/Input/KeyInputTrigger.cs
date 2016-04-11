using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class Key: InputEntity<KeyCode> {
	public Key(KeyCode keyCode, InputPhase phase): base(keyCode, phase) {
	}

	public static Key Canceled(KeyCode keyCode) {
		return new Key (keyCode, InputPhase.Canceled);
	}
}

public interface IKeyInput {
	KeyCode[] HandledKeys {get;}
	IObservable<IObservable<Key>> KeysObservable { get;}
}

public class KeyInputTrigger : ObservableTriggerBase, IKeyInput {

	[SerializeField]
	private KeyCode[] handledKeys;
	public KeyCode[] HandledKeys {get {return handledKeys;}}

	private InputTrigger<KeyCode, Key> keyTrigger;
	public IObservable<IObservable<Key>> KeysObservable  {get {return keyTrigger.InputObservables;}}

	void Awake() {
		handledKeys = handledKeys == null? new KeyCode[0]: handledKeys.Distinct ().ToArray ();
		keyTrigger = new InputTrigger<KeyCode, Key> (() => InputExt.KeysFromCodes (handledKeys), Key.Canceled);
	}

	void OnApplicationFocus(bool focus) {
		if (!focus) {
			keyTrigger.CancelAllInput();
		}
	}

	void OnDisable() {
		keyTrigger.CancelAllInput();
	}

	protected override void RaiseOnCompletedOnDestroy() {
		keyTrigger.CancelDestroyAllInput();
	}

	void Update() {
		keyTrigger.Update ();
	}

}
