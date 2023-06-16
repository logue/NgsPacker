// -----------------------------------------------------------------------
// <copyright file="DataContextDisposeAction.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace NgsPacker;

/// <summary>
///     アプリケーション終了時にデータコンテキストを破棄するクラス
/// </summary>
/// <see href="https://elf-mission.net/programming/wpf/getting-started-2020/step05/" />
public class DataContextDisposeAction : TriggerAction<FrameworkElement>
{
    /// <inheritdoc />
    protected override void Invoke(object parameter)
    {
        (AssociatedObject?.DataContext as IDisposable)?.Dispose();
    }
}
