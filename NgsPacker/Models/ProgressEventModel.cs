// -----------------------------------------------------------------------
// <copyright file="ProgressModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows;

namespace NgsPacker.Models;

/// <summary>
///     進捗情報
/// </summary>
public class ProgressEventModel
{
    private int current;
    private int max;
    private int min;
    private Visibility visibility;

    /// <summary>
    /// ダイアログの表示制御
    /// </summary>
    public bool IsVisible {
        get
        {
            return visibility == Visibility.Visible;
        }
        set
        {
            visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
    }

    /// <summary>
    ///     進捗値
    /// </summary>
    public int Value
    {
        get => current;
        set
        {
            if (current < min || current > max)
            {
                throw new IndexOutOfRangeException("Value must be smaller than Max and larger than Min value");
            }

            current = value;
        }
    }

    /// <summary>
    ///     最小値
    /// </summary>
    public int Min
    {
        get => min;
        set
        {
            if (min > max)
            {
                throw new IndexOutOfRangeException("Min value must be smaller than Max value");
            }

            IsIntermediate = min == max;
            min = value;
        }
    }

    /// <summary>
    ///     最大値
    /// </summary>
    public int Max
    {
        get => max;
        set
        {
            if (max < min)
            {
                throw new IndexOutOfRangeException("Max value must be larger than Min value");
            }

            IsIntermediate = min == max;
            max = value;
        }
    }

    /// <summary>
    ///     タイトル
    /// </summary>
    public string Title { get; set; } = "Now Loading...";

    /// <summary>
    ///     進捗テキスト
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     中間状態
    /// </summary>
    public bool IsIntermediate { get; set; }
}
