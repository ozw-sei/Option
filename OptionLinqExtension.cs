using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Option;

/// <summary>
/// corefx からInterfaceぱくった
/// </summary>
public static class OptionalLinqExtension
{

	public static IOption<TSource> FirstOption<TSource>(this IEnumerable<TSource> source) where TSource: class
	{
		return FirstOption(source, s => true); 
	}

	/// <summary>
	/// value is defined => Some<T>
	/// defualt => None<T>
	/// </summary>
	/// <returns>The option.</returns>
	/// <param name="source">Source.</param>
	/// <param name="f">F.</param>
	/// <typeparam name="TSource">The 1st type parameter.</typeparam>
	public static IOption<TSource> FirstOption<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> f) where TSource: class
	{
		var result = source.FirstOrDefault(f);
		if (result == null)
		{
			return Option<TSource>.None();
		}
		return Option<TSource>.Some(result);
	}

	public static IOption<TResult> Select<TSource, TResult>(this IOption<TSource> source, Func<TSource, TResult> selector)
	{
		if (selector == null)
			throw new ArgumentNullException();
		return source.Map(selector);
	}

	public static IOption<TResult> SelectMany<TSource, TResult>(this IOption<TSource> source, Func<TSource, IOption<TResult>> selector)
	{
		if (selector == null)
			throw new ArgumentNullException();
		return source.FlatMap(selector);
	}

	public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<IOption<TSource>> source, Func<IOption<TSource>, TResult> selector)
	{
		if (selector == null)
			throw new ArgumentNullException();

		foreach (var elem in source)
			yield return selector(elem);

	}

	public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<IOption<TSource>> source, Func<TSource, TResult> selector)
	{
		if (selector == null)
			throw new ArgumentNullException();

		foreach (var elem in source)
			if (elem.IsDefined)
				yield return selector(elem.Get());
	}

	public static IEnumerable<TResult> Zip<TSource1, TSource2, TSource3, TResult>(
		this IOption<TSource1> source, 
		IOption<TSource2> opt, 
		IOption<TSource3> opt2, 
		Func<TSource1, TSource2, TSource3, TResult> selector)
	{
		if (source.IsDefined && opt.IsDefined && opt2.IsDefined)
			yield return selector(source.Get(), opt.Get(), opt2.Get());
		
	}


	public static IEnumerable<UniRx.Tuple<TSource1, TSource2>> Zip<TSource1, TSource2>(this IOption<TSource1> source, IOption<TSource2> opt)
	{
		if (source.IsDefined && opt.IsDefined)
			yield return UniRx.Tuple.Create(source.Get(), opt.Get());
	}

	public static bool HasValue<TSource>(this IOption<TSource> source)
	{
		return source.IsDefined;
	}

	public static bool Any<TSource>(this IOption<TSource> source, Func<TSource, bool> predicate)
	{
		return source.Exists(predicate);
	}

	//	public static IOption<TResult> SelectMany<TSource, TCollection, TResult> (
	//		this IOption<TSource> source,
	//		Func<TSource, IOption<TCollection>> collectionSelector,
	//		Func<TSource, TCollection, TResult> resultSelector)
	//	{
	//		if (collectionSelector == null)
	//			throw new ArgumentNullException ();
	//		if (resultSelector == null)
	//			throw new ArgumentNullException ();
	//		return source.FlatMap (src => collectionSelector (src).Map (elem => resultSelector (src, elem)));
	//	}


	public static IOption<TSource> Where<TSource>(this IOption<TSource> source, Func<TSource, bool> predicate)
	{
		if (predicate == null)
			throw new ArgumentNullException();
		return source.Filter(predicate);
	}

	// TODO Any/ All/ Max/ Min/



}
