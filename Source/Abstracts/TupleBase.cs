﻿using System;
using System.Collections;

namespace KeepCoding
{
    /// <summary>
    /// An abstract type meant for the tuple data type for C# 4.
    /// </summary>
    public abstract class TupleBase : ITuple
    {
        /// <summary>
        /// Indexable tuple. Be careful when using this as the compiler will not notice if you are using the wrong type.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="UnrecognizedTypeException"></exception>
        /// <param name="index">The index to use.</param>
        /// <returns>The item indexed into.</returns>
        public object this[byte index] { get => throw IndexOutOfRange(index); set => throw IndexOutOfRange(index); }

        /// <summary>
        /// Determines if the tuple data type is empty.
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Gets the length of the tuple, describing the amount of elements there are.
        /// </summary>
        public byte Length => (byte)GetType().GetGenericArguments().Length;

        /// <summary>
        /// Gets the upper bound of the tuple, which is the last index.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public byte UpperBound => IsEmpty ? throw new InvalidOperationException("The tuple is empty, meaning that the upper bound doesn't exist.") : (byte)(Length - 1);

        /// <summary>
        /// All of the tuple's items as an array, ordered by item number.
        /// </summary>
        public abstract object[] ToArray { get; }

        /// <summary>
        /// Gets the enumerator of the tuple, using <see cref="ToArray"/>.
        /// </summary>
        /// <returns>All of the items in tuple.</returns>
        public IEnumerator GetEnumerator() => ToArray.GetEnumerator();

        private protected static TOutput Cast<TInput, TOutput>(in TInput value, in int index) => value is TOutput t ? t : throw UnrecognizedType(value, typeof(TOutput), index);

        private IndexOutOfRangeException IndexOutOfRange(in int i) => new IndexOutOfRangeException($"The index {i} was out of range from the tuple of length {ToArray.Length}.");

        private static UnrecognizedTypeException UnrecognizedType<T>(in T received, in Type expected, in int index) => new UnrecognizedTypeException($"The {(index + 1).ToOrdinal()} element in the tuple cannot be assigned because the value {received.UnwrapToString()} is type {received.GetType().Name} which doesn't match the expected type {expected.Name}.");
    }
}
