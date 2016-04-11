using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public enum InputPhase {
	Began,
	Hold,
	Ended,
	Canceled
}

public abstract class InputEntity<TKey> {
	public readonly TKey key;
	public readonly InputPhase phase;
	public InputEntity(TKey key, InputPhase phase) {
		this.key = key;
		this.phase = phase;
	}
}

public class InputTrigger<TKey, T> where T: InputEntity<TKey> {

	private readonly Func<IEnumerable<T>> updateInjection;
	private readonly Func<TKey, T> canceledInput;

	private Subject<IObservable<T>> subject;
	private Dictionary<TKey, Subject<T>> activeSubjects = new Dictionary<TKey, Subject<T>>();

	public IObservable<IObservable<T>> InputObservables {get { return FuncExt.CacheProperty (ref subject);}}

	public InputTrigger(Func<IEnumerable<T>> updateInjection, Func<TKey, T> canceledInput) {
		this.updateInjection = updateInjection ?? Enumerable.Empty<T>;
		this.canceledInput = canceledInput ?? (_ => default(T));
	}

	public void Update() {
		if (subject == null) {
			return;
		}
		if (!subject.HasObservers) {
			CancelDestroyAllInput ();
			return;
		}
		foreach (var inp in updateInjection()) {
			OnInject (inp);
		}
	}

	public void CancelDestroyAllInput() {
		CancelAllInput ();
		SubjectExt.CallCompleteDestroy (ref subject);
	}

	public void CancelAllInput() {
		foreach (var p in activeSubjects) {
			p.Value.OnNext (canceledInput(p.Key));
			p.Value.OnCompleted ();
		}
		activeSubjects.Clear ();
	}

	void OnInject(T inp) {
		switch (inp.phase) {
		case InputPhase.Began: 
			OnInputBegan (inp); return;
		case InputPhase.Hold: 
			OnInputHold (inp); return;
		case InputPhase.Canceled:
		case InputPhase.Ended: 
			OnInputEndedOrCanceled (inp); return;
		}
	}

	void OnInputBegan(T inp) {
		Subject<T> subj;
		if (activeSubjects.TryGetValue (inp.key, out subj)) {
			subj.OnNext (canceledInput(inp.key));
			subj.OnCompleted ();
		}
		subj = new Subject<T> ();
		activeSubjects [inp.key] = subj;
		subject.OnNext (subj);
		subj.OnNext (inp);
	}


	void OnInputHold(T inp) {
		Subject<T> subj;
		if (!activeSubjects.TryGetValue (inp.key, out subj)) {
			return;
		}
		subj.OnNext (inp);
	}

	void OnInputEndedOrCanceled(T inp) {
		Subject<T> subj;
		if (!activeSubjects.TryGetValue (inp.key, out subj)) {
			return;
		}
		subj.OnNext (inp);
		subj.OnCompleted ();
		activeSubjects.Remove (inp.key);
	}

}
