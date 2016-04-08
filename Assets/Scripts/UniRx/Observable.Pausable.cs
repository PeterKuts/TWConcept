using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx
{
	public static partial class ObservableEx
	{
		public static IObservable<T> Pausable<T>(this IObservable<T> source, IObservable<bool> pauser, bool initiallyRunning = false) 
		{
			return new PausableObservable<T>(source, pauser, initiallyRunning);
		}
	}
}