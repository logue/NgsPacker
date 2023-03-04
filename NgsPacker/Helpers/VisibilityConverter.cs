// -----------------------------------------------------------------------
// <copyright file="VisibilityConverter.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NgsPacker.Helpers;

/// <inheritdoc />
public class VisibilityConverter : IValueConverter
{
    /// <summary>
    ///     bool型からVisibility型に変換
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="targetType">enum型</param>
    /// <param name="parameter">パラメータ</param>
    /// <param name="culture">カルチャー</param>
    /// <returns>Enumの数値</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Binding.DoNothing;
        }

        return (bool)value ? Visibility.Visible : Visibility.Hidden;
    }

    /// <summary>
    ///     Visibility型からboolに変換
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="targetType">enum型</param>
    /// <param name="parameter">パラメータ</param>
    /// <param name="culture">カルチャー</param>
    /// <returns>Enumの数値</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Binding.DoNothing;
        }

        return value.Equals(true) ? parameter : Binding.DoNothing;
    }
}
