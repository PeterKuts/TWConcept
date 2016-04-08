using UnityEngine;
using System;

public static class MonoBehaviourExtension {

	public static T CacheComponent<T>(this MonoBehaviour behaviour, ref T prop) {
		if (prop == null) {
			prop = behaviour.GetComponent<T>();
		}
		return prop;
	}

	public static T[] CacheComponents<T>(this MonoBehaviour behaviour, ref T[] prop) {
		if (prop == null) {
			prop = behaviour.GetComponents<T>();
		}
		return prop;
	}

}
