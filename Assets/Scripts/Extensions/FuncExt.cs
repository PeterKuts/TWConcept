using System;
using UniRx;

public static class FuncExt {

	public static T CacheProperty<T>(ref T t, Func<T> create) where T: class {
		if (t == null) {
			t = create();
		}
		return t;
	}

	public static T CacheProperty<T>(ref T t) where T: class, new() {
		if (t == null) {
			t = new T();
		}
		return t;
	}

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

}
