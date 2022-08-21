// -----------------------------------------------------------------------
// <copyright file="ProgressMessage.cs" company="Logue">
// Copyright (c) 2022 Masashi Yoshikawa All rights reserved.
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
    /// <summary>
    /// メッセージの値が変化した
    /// </summary>
    public class SetMessage : PubSubEvent<string>
    {
    }
    /// <summary>
    /// タイトルの値が変化した
    /// </summary>
    public class SetTitle : PubSubEvent<string>
    {
    }
    /// <summary>
    /// 中間状態の値が変化した
    /// </summary>
    public class SetIntermediate : PubSubEvent<bool>
    {
    }
}
