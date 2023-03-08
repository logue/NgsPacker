// -----------------------------------------------------------------------
// <copyright file="ProgressEventModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using static NgsPacker.Helpers.ProgressDialog;

namespace NgsPacker.Models;

/// <summary>
///     進捗情報
/// </summary>
public class ProgressEventModel
{
    /// <summary>
    ///     ダイアログの表示制御
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    ///     進捗値
    /// </summary>
    public uint Value { get; set; }

    /// <summary>
    ///     最大値
    /// </summary>
    public uint Maximum { get; set; }

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Title { get; set; } = AppAssemblyModel.Title;

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Caption { get; set; } = "Now Loading...";

    /// <summary>
    ///     進捗テキスト
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     詳細テキスト
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    ///     中間状態
    /// </summary>
    public bool IsIntermediate { get; set; }

    /// <summary>
    ///     アニメーション
    /// </summary>
    public PROGANI Animation { get; set; }
}
