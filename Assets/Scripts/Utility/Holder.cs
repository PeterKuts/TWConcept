using UnityEngine;
using System.Collections;

public class Holder : MonoBehaviour {

	private static Holder sharedHolder;
	public static Holder SharedHolder {get {return sharedHolder;}}

	void Awake() {
		if (sharedHolder != null) {
			DestroyImmediate (this);
			return;
		}
		sharedHolder = this;
		GameObject.DontDestroyOnLoad (gameObject);
	}

}
