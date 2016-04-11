using UnityEngine;
using System.Collections;

public class Holder : MonoBehaviour {

	private static Holder sharedHolder;
	public static Holder SharedHolder {get {return sharedHolder;}}

	[SerializeField]
	private PointerInputTrigger pointerInput;
	public IPointerInput PointerInput {get {return  pointerInput;}}


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
