using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class RayBody : ObservableTriggerBase {

	[SerializeField]
	private Rigidbody2D _rigidbody;
	public Rigidbody2D Rigidbody {get {return FuncExt.CacheProperty(ref _rigidbody);}}
	public LayerMask layerMask = -1;
	public float minSqrExtent = 0.1f;

	private Vector2 previousPosition;
	private Subject<RaycastHit2D> subject;
	public IObservable<RaycastHit2D> RayCastObservable {get { return FuncExt.CacheProperty (ref subject);}}

	void Awake() {
		previousPosition = Rigidbody.position;
	}

	void FixedUpdate()
	{
		if (subject == null) {
			return;
		}
		if (!subject.HasObservers) {
			CompleteDestroySubject ();
			return;
		}

		Vector2 movementThisStep = Rigidbody.position - previousPosition; 
		float movementSqrMagnitude = movementThisStep.sqrMagnitude;

		if (movementSqrMagnitude <= minSqrExtent) {
			previousPosition = Rigidbody.position;
			return;
		}

		float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
		RaycastHit2D hitInfo = Physics2D.Raycast(previousPosition, movementThisStep, movementMagnitude, layerMask.value);

		if (hitInfo.collider == null) {
			previousPosition = Rigidbody.position;
			return;
		}

		subject.OnNext (hitInfo);

		previousPosition = Rigidbody.position;
	}

	void CompleteDestroySubject() {
		SubjectExt.CallCompleteDestroy (ref subject);
	}

	protected override void RaiseOnCompletedOnDestroy() {
		CompleteDestroySubject();
	}

}
