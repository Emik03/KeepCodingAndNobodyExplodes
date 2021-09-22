﻿using System;

namespace KeepCoding.Internal
{
    /// <summary>
    /// An exception thrown when an assertion fails.
    /// </summary>
    [Serializable]
    public sealed class AssertionException : KeepCodingException
    {
        /// <summary>
        /// An exception thrown when an assertion fails.
        /// </summary>
        public AssertionException() { }

        /// <summary>
        /// An exception thrown when an assertion fails.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public AssertionException(string message) : base(message) { }

        /// <summary>
        /// An exception thrown when an assertion fails.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">An <see cref="Exception"/> within this exception.</param>
        public AssertionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// An exception thrown when an assertion fails.
        /// </summary>
        /// <param name="received">The received value.</param>
        /// <param name="expected">The value of the expected value.</param>
        public AssertionException(object received, object expected) : base($"Expected {expected}, received: {received}.") { }
    }
}
