using System;
using UniRx;

public static class SubjectExt {
	
	public static void CallNext<T>(Subject<T> subject, Func<T> func) {
		if (subject == null) { return;}
		subject.OnNext (func ());
	}

	public static void CallNext<T>(Subject<T> subject, Func<bool> predicate, Func<T> func) {
		if (subject == null || predicate() == false) { return;}
		subject.OnNext (func ());
	}

	public static void CallComplete<T>(Subject<T> subject) {
		if (subject == null) { return;}
		subject.OnCompleted ();
	}

	public static void CallCompleteDestroy<T>(ref Subject<T> subject) {
		if (subject == null) { return;}
		subject.OnCompleted ();
		subject = null;
	}

}
