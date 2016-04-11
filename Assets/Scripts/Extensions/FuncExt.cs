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

	public static void OptionalCall<T>(T obj, Action<T> action) {
		if (obj == null || action == null) { return;}
		action (obj);
	}

	public static void OptionalIfCall<T>(T obj, Func<T, bool> predicate, Action<T> action) {
		if (obj == null || action == null || predicate == null || !predicate(obj)) { return; } 
		action (obj);
	}

}
