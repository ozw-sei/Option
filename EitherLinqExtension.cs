using System;
using System.Collections.Generic;
using System.Linq;
using Option;

public static class EitherLinqExtension
{
	public static IEither<TLeft, TResult> Select<TLeft, TRight, TResult> (this IEither<TLeft, TRight> either, Func<TRight, TResult> selector)
	{
		if (selector == null)
			throw new ArgumentNullException ();
		
		return either.Map (selector);
	}

	public static IEnumerable<TResult> SelectMany<TLeft, TRight, TResult> (this IEnumerable<IEither<TLeft, TRight>> eithers, Func<TRight, TResult> f)
	{
		if (f == null)
			throw new ArgumentNullException ();

		foreach (var e in eithers)
			if (e.IsRight)
				yield return f (e.Right);
	}
}
