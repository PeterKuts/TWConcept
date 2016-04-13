using UnityEngine;
using System.Collections;

public class BasePhys : MonoBehaviour {

	[SerializeField]
	private Transform _transform;
	public Transform Transform {get { return this.CacheComponent<Transform>(ref _transform); }}

	[SerializeField]
	private Rigidbody2D _rigidbody;
	public Rigidbody2D Rigidbody {get {return this.CacheComponent<Rigidbody2D>(ref _rigidbody);}}

	[SerializeField]
	private Collider2D _collider;
	public Collider2D Collider {get {return this.CacheComponent<Collider2D>(ref _collider);}}

}
