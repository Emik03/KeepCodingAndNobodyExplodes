﻿using System.Collections;
using System.Collections.Generic;

namespace KeepCoding
{
    /// <summary>
    ///     Encapsulates information about a group generated by <see
    ///     cref="UtilityHelper.GroupConsecutive{TItem}(IEnumerable{TItem}, IEqualityComparer{TItem})"/> and its
    ///     overloads.</summary>
    /// <typeparam name="TItem">
    ///     Type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">
    ///     Type of the key by which elements were compared.</typeparam>
    public sealed class ConsecutiveGroup<TItem, TKey> : IEnumerable<TItem>
    {
        private readonly IEnumerable<TItem> _group;

        internal ConsecutiveGroup(int index, List<TItem> group, TKey key)
        {
            Index = index;
            Count = group.Count;
            Key = key;
            _group = group;
        }

        /// <summary>Index in the original sequence where the group started.</summary>
        public int Index { get; private set; }

        /// <summary>Size of the group.</summary>
        public int Count { get; private set; }

        /// <summary>The key by which the items in this group are deemed equal.</summary>
        public TKey Key { get; private set; }

        /// <summary>Returns a string that represents this group’s key and its count.</summary>
        public override string ToString() => $"{Key}; Count = {Count}";

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<TItem> GetEnumerator() => _group.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
