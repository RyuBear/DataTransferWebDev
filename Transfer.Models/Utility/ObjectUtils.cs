using System;

namespace Utility
{
    public static class ObjectUtils
    {
        public static IOption<T> Opt<T>(this T obj)
        {
            return Option.Create(obj);
        }
        public static TResult OptGet<TSource, TResult>(this TSource obj, Func<TSource, TResult> selector, TResult defaultResult = default(TResult))
        {
            return obj.Opt().Select(selector).GetOrDefault(defaultResult);
        }
        public static T? To<T>(this string input) where T : struct, IConvertible
        {
            try
            {
                return (T?)Convert.ChangeType(input, typeof(T));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public static class Option
    {
        public static IOption<T> Some<T>(T value)
        {
            return new Some<T>(value);
        }
        public static IOption<T> None<T>()
        {
            return new None<T>();
        }
        public static IOption<T> Create<T>(T obj)
        {
            return Equals(obj, default(T)) ? None<T>() : Some(obj);
        }
        public static IOption<TResult> Select<TSource, TResult>(this IOption<TSource> source, Func<TSource, TResult> selector)
        {
            return source.HasValue ? Some(selector(source.Value)) : None<TResult>();
        }
        public static T GetOrDefault<T>(this IOption<T> source, T defaultValue = default(T))
        {
            return source.HasValue ? source.Value : defaultValue;
        }
    }
    public interface IOption<out T>
    {
        bool HasValue { get; }
        T Value { get; }
    }
    public class Some<T>: IOption<T>
    {
        public bool HasValue { get { return true; } }
        public T Value { get; private set; }
        public Some(T value) { Value = value; }
    }
    public class None<T> : IOption<T>
    {
        public bool HasValue { get { return false; } }
        public T Value { get { throw new NullReferenceException(); } }
    }
}