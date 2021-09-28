// -----------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using Prism.Events;

namespace NgsPacker.Services
{
    /// <summary>
    /// Defines the <see cref="MessageService" />.
    /// </summary>
    public class MessageService : PubSubEvent<string>
    {
    }
}