using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GizmoDrawer : MonoBehaviour {

	public Color color = Color.white;

	private Transform cTransform = null;
	private Transform Transform {get { return this.CacheComponent<Transform> (ref cTransform);}}

	private CircleCollider2D[] circleColliders = null;
	private BoxCollider2D[] boxColliders = null;
	private EdgeCollider2D[] edgeColliders = null;
	private PolygonCollider2D[] polygonColliders = null;
	private CircleCollider2D[] CircleColliders {get {return this.CacheComponents<CircleCollider2D>(ref circleColliders);}}
	private BoxCollider2D[] BoxColliders {get {return this.CacheComponents<BoxCollider2D>(ref boxColliders);}}
	private EdgeCollider2D[] EdgeColliders {get {return this.CacheComponents<EdgeCollider2D>(ref edgeColliders);}}
	private PolygonCollider2D[] PolygonColliders {get {return this.CacheComponents<PolygonCollider2D>(ref polygonColliders);}}

	public void ClearCache() {
		cTransform = null;
		circleColliders = null;
		boxColliders = null;
		edgeColliders = null;
		polygonColliders = null;
	}

	void OnDrawGizmos() {
		Handles.color = color;
		try {
			foreach (var c in CircleColliders) {
				DrawGizmo (c);
			}
			foreach (var c in BoxColliders) {
				DrawGizmo (c);
			}
			foreach (var c in EdgeColliders) {
				DrawGizmo (c);
			}
			foreach (var c in PolygonColliders) {
				DrawGizmo (c);
			}
		} catch {
			ClearCache ();
		}
	}

	void OnDrawGizmosSelected() {
	}

	Vector3 ColliderPosition(Vector3 pos, Vector2 offset) {
		return new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z);
	}

	void DrawGizmo(BoxCollider2D collider) {
		Rect rect = new Rect (ColliderPosition(Transform.position, collider.offset), collider.offset);
		Handles.DrawSolidRectangleWithOutline (rect, color, color);
	}

	void DrawGizmo(CircleCollider2D collider) {
		Handles.DrawSolidDisc(ColliderPosition(Transform.position, collider.offset), Vector3.forward, collider.radius);
	}

	void DrawGizmo(EdgeCollider2D collider) {
		var points = collider.points.Select (v => {
			return ColliderPosition(Transform.position, v);
		});
		Handles.DrawPolyLine (points.ToArray ());
	}

	void DrawGizmo(PolygonCollider2D collider) {
		var points = collider.points.Select (v => {
			return ColliderPosition(Transform.position, v);
		});
		Handles.DrawPolyLine (points.ToArray ());
	}

}

[CustomEditor(typeof(GizmoDrawer))]
[CanEditMultipleObjects]
class GizmoDrawerEditor: Editor {

	public override void OnInspectorGUI() {
		GizmoDrawer obj = serializedObject.targetObject as GizmoDrawer;
		if (obj == null) {
			return;
		}
		obj.color = EditorGUILayout.ColorField(obj.color);
		if (GUILayout.Button ("Refresh")) {
			obj.ClearCache ();
		}
	}
}
