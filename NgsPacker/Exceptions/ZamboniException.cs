// -----------------------------------------------------------------------
// <copyright file="ZamboniException.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
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
        public ZamboniException(Exception e) : base()
        {
        }

        public ZamboniException(string message) : base(message)
        {
        }

        public ZamboniException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZamboniException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}