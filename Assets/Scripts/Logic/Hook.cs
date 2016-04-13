using UnityEngine;
using System.Collections;
using UniRx;

public class Hook : BasePhys {

	[SerializeField]
	private RayBody rayBody;

	void Awake() {
		rayBody.RayCastObservable.Subscribe(OnHit);
	}

	void OnHit(RaycastHit2D hit) {
		Rigidbody.position = hit.point;
		Rigidbody.velocity = Vector2.zero;
	}
}
