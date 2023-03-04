// -----------------------------------------------------------------------
// <copyright file="ProgressDialogViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Events;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Windows;

namespace NgsPacker.ViewModels;

/// <summary>
/// 進捗ダイアログのビューモデル
/// </summary>
public class ProgressDialogViewModel : BindableBase, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressDialogViewModel" /> class.
    /// </summary>
    public ProgressDialogViewModel(IEventAggregator eventAggregator, ILocalizeService localizeService)
    {
        eventAggregator
            .GetEvent<ProgressEvent>()
            .Subscribe(ProgressEventHandle, ThreadOption.PublisherThread, true);

        Title = localizeService.GetLocalizedString("ProgressDialogText");
        this.eventAggregator = eventAggregator;
    }

    /// <summary>
    ///     ダイアログの表示制御
    /// </summary>
    public bool DialogVisibility { get; set; } = true;

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     メッセージ
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    ///     最小値
    /// </summary>
    public int Minimum { get; set; } = 100;

    /// <summary>
    ///     最大値
    /// </summary>
    public int Maximum { get; set; }

    /// <summary>
    ///     進捗
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    ///     キャンセル可能か
    /// </summary>
    public bool Closable { get; set; }

    /// <summary>
    ///     進捗リングの表示制御
    /// </summary>
    public Visibility ProgressRingVisibility { get; set; } = Visibility.Visible;

    /// <summary>
    ///     進捗バーの表示制御
    /// </summary>
    public Visibility ProgressBarVisibility { get; set; } = Visibility.Hidden;

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
        eventAggregator
            .GetEvent<ProgressEvent>()
            .Unsubscribe(ProgressEventHandle);
    }

    /// <summary>
    /// イベントアグリエイター
    /// </summary>
    private readonly IEventAggregator eventAggregator;

    /// <summary>
    ///     進捗モデルのイベントハンドラ
    /// </summary>
    /// <param name="model">進捗モデル</param>
    private void ProgressEventHandle(ProgressEventModel model)
    {
        Debug.WriteLine(model.Message);
        Title = model.Title;
        Message = model.Message;
        Minimum = model.Min;
        Maximum = model.Max;
        Value = model.Value;
        DialogVisibility = model.IsVisible;

        if (model.IsIntermediate)
        {
            ProgressRingVisibility = Visibility.Visible;
            ProgressBarVisibility = Visibility.Hidden;
        }
        else
        {
            ProgressRingVisibility = Visibility.Hidden;
            ProgressBarVisibility = Visibility.Visible;
        }
    }
}
