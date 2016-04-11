using UnityEngine;
using System;

public enum PointerPhase {
	Began,
	Moved,
	Ended,
	Canceled
}

public class Pointer {
	public readonly int id;
	public readonly Vector3 position;
	public readonly PointerPhase phase;
	public Pointer(int id, Vector3 position, PointerPhase phase) {
		this.id = id;
		this.position = position;
		this.phase = phase;
	}

	public static Pointer Canceled(int id) {
		return new Pointer (id, Vector3.zero, PointerPhase.Canceled);
	}
}

