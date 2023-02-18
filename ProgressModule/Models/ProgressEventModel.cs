// -----------------------------------------------------------------------
// <copyright file="ProgressEventModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Events;
using Prism.Mvvm;

namespace NgsPacker.Models;

/// <summary>
///     進捗モデル
/// </summary>
public class ProgressEventModel : BindableBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressEventModel" /> class.
    ///     コンストラクタ
    /// </summary>
    /// <param name="eventAggregator">イベント</param>
    public ProgressEventModel(IEventAggregator eventAggregator)
    {
        Progress = 0;

        _ = eventAggregator
            .GetEvent<SetTitle>()
            .Subscribe(x => Title = x);
        _ = eventAggregator
            .GetEvent<SetMessage>()
            .Subscribe(x => Message = x);
        _ = eventAggregator
            .GetEvent<SetIntermediate>()
            .Subscribe(x => IsIntermediate = x);
        _ = eventAggregator
            .GetEvent<SetProgress>()
            .Subscribe(x => Progress = x);
    }

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     メッセージ
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     進捗
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    ///     中間状態（リンクを表示）
    /// </summary>
    public bool IsIntermediate { get; set; }
}