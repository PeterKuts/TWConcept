using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public interface IPointerInput {
	IObservable<IObservable<Pointer>> PointersAsObservable { get;}
}

public class PointerInputTrigger : ObservableTriggerBase, IPointerInput {

	private Subject<IObservable<Pointer>> pointerSubjs;
	private Dictionary<int, Subject<Pointer>> activePointerSubjs = new Dictionary<int, Subject<Pointer>>();
	private Func<IEnumerable<Pointer>> GetPointers;

	public IObservable<IObservable<Pointer>> PointersAsObservable  {get {return FuncExt.CacheProperty (ref pointerSubjs);}}

	void Start() {
		if (Input.touchSupported) {
			GetPointers = InputExt.TouchesPointers;
		} else if (Input.mousePresent) {
			GetPointers = InputExt.MousePointers;
		} else {
			GetPointers = Enumerable.Empty<Pointer>;
		}
	}

	void OnApplicationFocus(bool focus) {
		if (!focus) {
			CancelAllPointers ();
		}
	}

	void OnDisable() {
		CancelAllPointers ();
	}

	protected override void RaiseOnCompletedOnDestroy() {
		CancelDestroyAllPointers();
	}

	void Update() {
		if (pointerSubjs == null) {
			return;
		}
		if (!pointerSubjs.HasObservers) {
			CancelDestroyAllPointers ();
			return;
		}
		foreach (var p in GetPointers()) {
			OnPointer (p);
		}
	}

	void OnPointer(Pointer p) {
		switch (p.phase) {
		case PointerPhase.Began: 
			OnPointerBegan (p); return;
		case PointerPhase.Moved: 
			OnPointerMoved (p); return;
		case PointerPhase.Canceled:
		case PointerPhase.Ended: 
			OnPointerEndedOrCanceled (p); return;
		}
	}

	void OnPointerBegan(Pointer p) {
		Subject<Pointer> subj;
		if (activePointerSubjs.TryGetValue (p.id, out subj)) {
			subj.OnNext (Pointer.Canceled(p.id));
			subj.OnCompleted ();
		}
		subj = new Subject<Pointer> ();
		activePointerSubjs [p.id] = subj;
		pointerSubjs.OnNext (subj);
		subj.OnNext (p);
	}

	void OnPointerMoved(Pointer p) {
		Subject<Pointer> subj;
		if (!activePointerSubjs.TryGetValue (p.id, out subj)) {
			return;
		}
		subj.OnNext (p);
	}

	void OnPointerEndedOrCanceled(Pointer p) {
		Subject<Pointer> subj;
		if (!activePointerSubjs.TryGetValue (p.id, out subj)) {
			return;
		}
		subj.OnNext (p);
		subj.OnCompleted ();
		activePointerSubjs.Remove (p.id);
	}

	void CancelDestroyAllPointers() {
		CancelAllPointers ();
		SubjectExt.CallCompleteDestroy (ref pointerSubjs);
	}

	void CancelAllPointers() {
		foreach (var p in activePointerSubjs) {
			p.Value.OnNext (Pointer.Canceled(p.Key));
			p.Value.OnCompleted ();
		}
		activePointerSubjs.Clear ();
	}

}
