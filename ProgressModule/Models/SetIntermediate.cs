// -----------------------------------------------------------------------
// <copyright file="SetIntermediate.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Events;

namespace ProgressModule.Models
{
    /// <summary>
    /// 中間状態に変化した
    /// </summary>
    public class SetIntermediate : PubSubEvent<bool>
    {
    }
}