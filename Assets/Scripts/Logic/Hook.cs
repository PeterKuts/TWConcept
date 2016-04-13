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

	private CompositeDisposable disposables = new CompositeDisposable();

	public void Init(IObservable<Pointer> pointers, IObservable<Unit> fixedUpdate) {
		RayBody.RayCastObservable.Subscribe(OnHit).AddTo(disposables);
		pointers.Buffer (fixedUpdate).SelectMany (l => l).Subscribe (PoitnerOnNext).AddTo(disposables);
	}

	void OnDestroy() {
		disposables.Dispose ();
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
	}
}
