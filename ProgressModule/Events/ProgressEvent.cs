// -----------------------------------------------------------------------
// <copyright file="ProgressEvent.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Models;
using Prism.Events;

namespace NgsPacker.Events;

public class ProgressEvent : PubSubEvent<ProgressEventModel>
{
}
