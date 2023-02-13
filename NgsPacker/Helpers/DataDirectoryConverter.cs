// -----------------------------------------------------------------------
// <copyright file="DataDirectoryConverter.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace NgsPacker.Helpers
{
    /// <summary>
    /// ディレクトリ種別
    /// </summary>
    [Flags]
    public enum DataDirectoryType
    {
        /// <summary>
        /// PSO2ディレクトリのみ（win32, win32_na）
        /// </summary>
        Pso = 0b01, // 1

        /// <summary>
        /// NGSディレクトリ（win32reboot, win32reboot_na)
        /// </summary>
        Ngs = 0b10, // 2

        /// <summary>
        /// すべて
        /// </summary>
        All = 0b11, // 3
    }

    /// <inheritdoc />
    public class DataDirectoryConverter : IValueConverter
    {
        /// <summary>
        /// Enum型からboolに変換
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

            return value.Equals(parameter);
        }

        /// <summary>
        /// bool型からEnumに変換
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
}
