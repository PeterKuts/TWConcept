using UnityEngine;
using System.Collections;
using UniRx;

public class Hook : MonoBehaviour {

	[SerializeField]
	private Transform _transform;
	public Transform Transform {get { return this.CacheComponent<Transform>(ref _transform); }}

	[SerializeField]
	private Rigidbody2D _rigidbody;
	public Rigidbody2D Rigidbody {get {return this.CacheComponent<Rigidbody2D>(ref _rigidbody);}}

	[SerializeField]
	private RayBody _rayBody;
	public RayBody RayBody {get {return this.CacheComponent<RayBody>(ref _rayBody);}}

	private Body body;
	private Joint2D joint;

	private CompositeDisposable disposables = new CompositeDisposable();

	public void Init(IObservable<Pointer> pointers, IObservable<Unit> fixedUpdate, Body body) {
		RayBody.RayCastObservable.First().Subscribe(OnHit).AddTo(disposables);
		pointers.Buffer (fixedUpdate).SelectMany (l => l).Subscribe (PoitnerOnNext).AddTo(disposables);
		this.body = body;
	}

	void OnDestroy() {
		disposables.Dispose ();
		GameObject.Destroy (joint);
	}

	void PoitnerOnNext(Pointer p) {
		switch (p.phase) {
		case InputPhase.Began:
			PointerBegan(p);
			return;
		case InputPhase.Hold:
			PointerMoved(p);
			return;
		case InputPhase.Ended: 
			PointerEnded(p);
			return;
		case InputPhase.Canceled: 
			PointerCanceled(p);
			return;
		}
	}

	void PointerBegan(Pointer p) {
		var pos = (Vector2)Camera.main.ScreenToWorldPoint (p.position);
		Rigidbody.velocity = (pos - Rigidbody.position).normalized * 100.0f;
	}

	void PointerMoved(Pointer p) {
	}

	void PointerEnded(Pointer p) {
		GameObject.Destroy (gameObject);
	}

	void PointerCanceled(Pointer p) {
		GameObject.Destroy (gameObject);
	}

	void OnHit(RaycastHit2D hit) {
		Rigidbody.position = hit.point;
		Rigidbody.velocity = Vector2.zero;
		var joint = body.gameObject.AddComponent<SpringJoint2D>();
		joint.enableCollision = true;
		joint.distance = Mathf.Max (body.Collider.bounds.extents.x, Mathf.Max (body.Collider.bounds.extents.y, body.Collider.bounds.extents.z));
		joint.connectedBody = hit.rigidbody;
		joint.connectedAnchor = hit.rigidbody != null? (Vector2)hit.transform.InverseTransformPoint (hit.point): hit.point;
		joint.frequency = 0;
		joint.dampingRatio = 1.0f;
		this.joint = joint;
	}
}
