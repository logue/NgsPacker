// -----------------------------------------------------------------------
// <copyright file="SetTitle.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Events;

namespace ProgressModule.Models;

/// <summary>
///     タイトルの値が変化した
/// </summary>
public class SetTitle : PubSubEvent<string>
{
}
