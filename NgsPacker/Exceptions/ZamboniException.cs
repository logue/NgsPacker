// -----------------------------------------------------------------------
// <copyright file="ZamboniException.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace NgsPacker.Exeptions
{
    /// <summary>
    /// Zamboni内で発生した例外
    /// </summary>
    internal class ZamboniException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZamboniException"/> class.
        /// </summary>
        /// <param name="e">Exception</param>
        public ZamboniException(Exception e)
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZamboniException"/> class.
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public ZamboniException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZamboniException"/> class.
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"><inheritdoc/></param>
        public ZamboniException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZamboniException"/> class.
        /// </summary>
        /// <param name="info"><inheritdoc/></param>
        /// <param name="context"><inheritdoc/></param>
        protected ZamboniException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}