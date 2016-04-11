using UnityEngine;
using System.Collections;

public class Holder : MonoBehaviour {

	private static Holder sharedHolder;
	public static Holder SharedHolder {get {return sharedHolder;}}

	[SerializeField]
	private PointerInputTrigger pointerInput;
	public IPointerInput PointerInput {get {return  pointerInput;}}

	[SerializeField]
	private KeyInputTrigger keyInput;
	public IKeyInput KeyInput {get {return  keyInput;}}

	void Awake() {
		if (sharedHolder != null) {
			Debug.LogWarning ("Can be only one Holder!");
			DestroyImmediate (this);
			return;
		}
		sharedHolder = this;
		GameObject.DontDestroyOnLoad (gameObject);
	}

}
