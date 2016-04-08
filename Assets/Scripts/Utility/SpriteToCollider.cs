using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class SpriteToCollider : MonoBehaviour {

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer.ObserveEveryValueChanged (sr => sr.sprite, FrameCountType.FixedUpdate).Subscribe(SetupAllColliders);
	}

	void SetupAllColliders(Sprite sprite) {
		foreach (var c in GetComponents<Collider2D>()) {
			if (SetupCollider (c as BoxCollider2D, sprite)) {
				continue;
			}
			if (SetupCollider (c as CircleCollider2D, sprite)) {
				continue;
			}
			if (SetupCollider (c as EdgeCollider2D, sprite)) {
				continue;
			}
			if (SetupCollider (c as PolygonCollider2D, sprite)) {
				continue;
			}
		}
	}

	bool SetupCollider(BoxCollider2D collider, Sprite sprite) {
		if (collider == null) {return false;}
		var bounds = sprite != null? sprite.bounds: new Bounds();
		collider.size = bounds.size;
		collider.offset = bounds.center;
		return true;
	}

	bool SetupCollider(CircleCollider2D collider, Sprite sprite) {
		if (collider == null) {return false;}
		var bounds = sprite != null? sprite.bounds: new Bounds();
		collider.radius = Mathf.Min (bounds.size.x, bounds.size.y) * 0.5f;;
		collider.offset = bounds.center;
		return true;
	}

	bool SetupCollider(EdgeCollider2D collider, Sprite sprite) {
		if (collider == null) {return false;}
		var bounds = sprite != null? sprite.bounds: new Bounds();
		collider.offset = bounds.center;
		var size = bounds.size * 0.5f;
		collider.points = new Vector2[5] {
			new Vector2(-size.x, -size.y),
			new Vector2(-size.x, size.y),
			new Vector2(size.x, size.y),
			new Vector2(size.x, -size.y),
			new Vector2(-size.x, -size.y),
		};
		return true;
	}

	bool SetupCollider(PolygonCollider2D collider, Sprite sprite) {
		if (collider == null) {return false;}
		var bounds = sprite != null? sprite.bounds: new Bounds();
		collider.offset = bounds.center;
		var size = bounds.size * 0.5f;
		collider.points = new Vector2[5] {
			new Vector2(-size.x, -size.y),
			new Vector2(-size.x, size.y),
			new Vector2(size.x, size.y),
			new Vector2(size.x, -size.y),
			new Vector2(-size.x, -size.y),
		};
		return true;
	}
}
