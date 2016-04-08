using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx.Operators
{
	internal class PausableObservable<TSource> : OperatorObservableBase<TSource>
	{
		readonly IObservable<TSource> source;
		readonly IObservable<bool> pauser;
		readonly bool initiallyRunning;

		public PausableObservable(IObservable<TSource> source, IObservable<bool> pauser, bool initiallyRunning)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.pauser = pauser;
			this.initiallyRunning = initiallyRunning;
		}

		protected override IDisposable SubscribeCore(IObserver<TSource> observer, IDisposable cancel)
		{
			return new Pausable(this, observer, cancel).Run();
		}

		class Pausable : OperatorObserverBase<TSource, TSource>
		{
			readonly PausableObservable<TSource> parent;
			object gate = new object();
			bool isRunning;

			public Pausable(PausableObservable<TSource> parent, IObserver<TSource> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isRunning = parent.initiallyRunning;

				var sourceSubscription = parent.source.Subscribe(this);
				var pauserSubscription = parent.pauser.Subscribe(new Pauser(this));

				return StableCompositeDisposable.Create(sourceSubscription, pauserSubscription);
			}

			public override void OnNext(TSource value)
			{
				lock (gate)
				{
					if (!isRunning) {
						return;
					}
				}
				observer.OnNext (value);
			}

			public override void OnError(Exception error)
			{
				lock (gate)
				{
					try { observer.OnError(error); } finally { Dispose(); }
				}
			}

			public override void OnCompleted()
			{
				lock (gate)
				{
					try { observer.OnCompleted(); } finally { Dispose(); }
				}
			}

			class Pauser : IObserver<bool>
			{
				readonly Pausable parent;

				public Pauser(Pausable parent)
				{
					this.parent = parent;
				}

				public void OnNext(bool value)
				{
					lock (parent.gate)
					{
						parent.isRunning = value;
					}
				}

				public void OnError(Exception error)
				{
					parent.OnError(error);
				}

				public void OnCompleted()
				{
					parent.OnCompleted();
				}
			}
		}
	}

}