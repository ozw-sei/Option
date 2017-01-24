using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Option
{
	/// <summary>
	/// Right Error
	/// </summary>
	public interface IEither<T1, T2>: IEnumerable<T2>
	{
		U Use<U> (Func<T1, U> ofLeft, Func<T2, U> ofRight);

		void Use (Action<T1> ofLeft, Action<T2> ofRight);

		IEither<T1, TResult> Map<TResult> (Func<T2, TResult> f);

		T2 GetOrElse (T2 defaultValue);

		IOption<T2> ToOption ();

		T1 Left {
			get;
		}

		T2 Right {
			get;
		}

		bool IsRight {
			get;
		}

		bool IsLeft {
			get;
		}

	}

	public static class Either
	{

		public static IEither<T1, T2> Left<T1, T2> (T1 value)
		{
			return new Either.LeftEither<T1, T2> (value);
		}

		public static IEither<T1, T2> Right<T1, T2> (T2 value)
		{
			return new Either.RightEither<T1, T2> (value);
		}



		sealed class LeftEither<T1, T2>: IEither<T1, T2>
		{
			private readonly T1 value;

			public LeftEither (T1 value)
			{
				this.value = value;
			}

			public IOption<T2> ToOption ()
			{
				return Option<T2>.None ();
			}


			public U Use<U> (Func<T1, U> ofLeft, Func<T2, U> ofRight)
			{
				if (ofLeft == null)
					throw new ArgumentNullException ();
				return ofLeft (value);
			}

			public void Use (Action<T1> ofLeft, Action<T2> ofRight)
			{
				if (ofLeft == null)
					throw new ArgumentNullException ();
				ofLeft (value);
			}

			public bool IsRight {
				get {
					return false;
				}
			}

			public bool IsLeft {
				get {
					return true;
				}
			}

			public T1 Left {
				get {
					return value;
				}
			}

			public T2 Right {
				get {
					throw new InvalidOperationException ();
				}
			}

			public T2 GetOrElse (T2 defaultValue)
			{
				return defaultValue;
			}


			public IEither<T1, TResult> Map<TResult> (Func<T2, TResult> f)
			{
				return Either.Left<T1, TResult> (value);
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return GetEnumerator ();
			}


			public IEnumerator<T2> GetEnumerator ()
			{
				if (IsRight) {
					yield return default(T2);
				}
			}
		}

		sealed class RightEither<T1, T2>: IEither<T1, T2>
		{
			private readonly T2 value;

			public RightEither (T2 value)
			{
				this.value = value;
			}

			public U Use<U> (Func<T1, U> ofLeft, Func<T2, U> ofRight)
			{
				if (ofRight == null)
					throw new ArgumentNullException ();
				return ofRight (value);
			}

			public void Use (Action<T1> ofLeft, Action<T2> ofRight)
			{
				if (ofRight == null)
					throw new ArgumentNullException ();
				ofRight (value);
			}

			public T2 GetOrElse (T2 defaultValue)
			{
				return value;
			}

			public IOption<T2> ToOption ()
			{
				return Option<T2>.Some (value);
			}

			public bool IsRight {
				get {
					return true;
				}
			}

			public bool IsLeft {
				get {
					return false;
				}
			}

			public T1 Left {
				get {
					throw new InvalidOperationException ();
				}
			}

			public T2 Right {
				get {
					return value;
				}
			}

			public IEither<T1, TResult> Map<TResult> (Func<T2, TResult> f)
			{
				return Either.Right<T1, TResult> (f (value));
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return GetEnumerator ();
			}


			public IEnumerator<T2> GetEnumerator ()
			{
				if (IsRight) {
					yield return value;
				}
			}

		}
	}
}

