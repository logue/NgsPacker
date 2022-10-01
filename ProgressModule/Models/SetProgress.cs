// -----------------------------------------------------------------------
// <copyright file="SetProgress.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Events;

namespace ProgressModule.Models
{
    /// <summary>
    /// 進捗の値が変化した
    /// </summary>
    public class SetProgress : PubSubEvent<int>
    {
    }
}
