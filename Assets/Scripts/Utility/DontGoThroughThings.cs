using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
	public LayerMask layerMask = -1; //make sure we aren't in this layer 
	public float skinWidth = 0.1f; //probably doesn't need to be changed 

	private float minimumExtent; 
	private float partialExtent; 
	private float sqrMinimumExtent; 
	private Vector2 previousPosition; 
	private Rigidbody2D myRigidbody;
	private Collider2D myCollider;

	//initialize values 
	void Start() 
	{ 
		myRigidbody = GetComponent<Rigidbody2D>();
		myCollider = GetComponent<Collider2D>();
		previousPosition = myRigidbody.position; 
		minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z); 
		partialExtent = minimumExtent * (1.0f - skinWidth); 
		sqrMinimumExtent = minimumExtent * minimumExtent; 
	} 

	void FixedUpdate() 
	{ 
		Vector2 movementThisStep = myRigidbody.position - previousPosition; 
		float movementSqrMagnitude = movementThisStep.sqrMagnitude;

		if (movementSqrMagnitude <= sqrMinimumExtent) {
			previousPosition = myRigidbody.position;
			return;
		}

		float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
		RaycastHit2D hitInfo = Physics2D.Raycast(previousPosition, movementThisStep, movementMagnitude, layerMask.value);

		if (hitInfo.collider == null) {
			previousPosition = myRigidbody.position;
			return;
		}

		if (hitInfo.collider.isTrigger) {
			Debug.Log ("Trigger");
		} else {
			Debug.Log ("Collider");
			myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 
		}
		previousPosition = myRigidbody.position;
	}
}