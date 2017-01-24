using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Option.
/// </summary>
namespace Option
{
	public struct IOption<T>: IEnumerable<T>, IEquatable<T>
	{

		readonly bool isDefined;
		readonly T value;

		public bool IsDefined {
			get {
				return isDefined;
			}
		}

		public T Get ()
		{
			if (isDefined)
				return value;
			else
				throw new NoneGetException ();
		}

		internal IOption (bool isDefined, T value)
		{
			this.isDefined = isDefined;
			this.value = value;
		}			

		public bool Equals (T other)
		{
			if (isDefined) {
				return EqualityComparer<T>.Default.Equals (value, other);
			}

			return false;
		}


		public override bool Equals (object obj)
		{
			return obj is IOption<T> ? Equals ((IOption<T>)obj) : false;
		}

		public override int GetHashCode ()
		{
			if (isDefined) {
				if (value == null) {
					return 1;
				}
				return value.GetHashCode ();
			}
			return 0;
		}

		public override string ToString ()
		{
			if (isDefined) {
				if (value == null) {
					return "Some(null)";
				}

				return string.Format ("Some({0})", value);
			}

			return "None";
		}

		public bool Contains (T value)
		{
			if (isDefined) {
				if (this.value == null) {
					return value == null;
				}

				return this.value.Equals (value);
			}

			return false;
		}

		public bool Exists (Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException ();
			return isDefined && predicate (value);
		}


		public T GetOrElse (T defaultValue)
		{
			if (isDefined) {
				return value;
			}
			return defaultValue;
		}

		public T GetOrElse (Func<T> altFunc)
		{
			if (altFunc == null)
				throw new ArgumentNullException ();

			if (isDefined)
				return value;

			return altFunc ();
		}

		public IOption<T> Else (IOption<T> alt)
		{
			if (isDefined)
				return this;

			return alt;				 
		}

		public IOption<T> Else (Func<IOption<T>> altFunc)
		{
			if (altFunc == null)
				throw new ArgumentNullException ();

			if (isDefined)
				return this;
							
			return altFunc ();	
		}

		public TResult Match<TResult> (Func<T, TResult> some, Func<TResult> none)
		{
			if (isDefined)
				return some (value);
			else
				return none ();
		}

		public void Match (Action<T> some, Action none)
		{
			if (some == null)
				throw new ArgumentNullException ();

			if (none == null)
				throw new ArgumentNullException ();

			if (isDefined)
				some (value);
			else
				none ();
		}

		public void Match(Func<T, bool> predicate, Action<T> some, Action none){
			if(isDefined && predicate(value)){
				some (value);
			}
			else{
				none ();
			}
		}

		public TResult Match<TResult>(Func<T, bool> predicate, Func<T, TResult> some, Func<TResult> none){
			if(isDefined && predicate(value)){
				return some (value);
			}
			else{
				return none ();
			}
		}
		

		public void MatchSome (Action<T> some)
		{
			if (some == null)
				throw new ArgumentNullException ();

			if (isDefined)
				some (value);
		}

		public void MatchNone (Action none)
		{
			if (none == null)
				throw new ArgumentNullException ();

			if (!isDefined)
				none ();
		}

		public IOption<TResult> Map<TResult> (Func<T, TResult> mapping)
		{
			if (isDefined)
				return Option<TResult>.Some (mapping (value));
			else
				return Option<TResult>.None ();
		}

			
		public IOption<TResult> FlatMap<TResult> (Func<T, IOption<TResult>> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException ();

			return Match<IOption<TResult>> (
				some: value => mapping (value),
				none: () => Option<TResult>.None ()
			);
		}

		public IOption<T> Filter (Func<T, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException ();

			if (isDefined && !predicate (value)) {
				return Option<T>.None ();
			} else {
				return this;
			}
				
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}


		/// <summary>
		/// Returns an enumerator for the optional.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator ()
		{
			if (isDefined) {
				yield return value;
			}
		}

		public IEither<TError, T> ToEither<TError> (TError error)
		{
			if (isDefined)
				return Either.Right<TError, T> (value);
			else
				return Either.Left<TError, T> (error);
		}
	}


	public static class Option<T>
	{
		public static IOption<T> Some (T theValue)
		{
			return new IOption<T> (true, theValue);
		}

		public static IOption<T> None ()
		{
			return new IOption<T> (false, default(T));
		}

		/// <summary>
		/// Null セーフにキャストする
		/// </summary>
		/// <returns>The opt.</returns>
		/// <param name="theValue">The value.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public static IOption<TResult> AsOpt<TResult>(T theValue) where TResult: class {
			var result = theValue as TResult;
			if(result == null)
				return Option<TResult>.None();
			return Option<TResult>.Some(result);
		}
	}
}

