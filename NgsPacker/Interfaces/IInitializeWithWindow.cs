// -----------------------------------------------------------------------
// <copyright file="IInitializeWithWindow.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace NgsPacker.Interfaces;

/// <summary>
///     WPFからMessageDialogを呼ぶ場合のおまじない
/// </summary>
/// <see href="https://qiita.com/okazuki/items/227f8d19e38a67099006" />
[ComImport]
[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IInitializeWithWindow
{
    /// <summary>
    ///     初期化
    /// </summary>
    /// <param name="hwnd">The hwnd<see cref="IntPtr" />.</param>
    void Initialize([In] IntPtr hwnd);
}

/// <summary>
///     Defines the <see cref="IWindowNative" />.
/// </summary>
[ComImport]
[Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowNative
{
    /// <summary>
    ///     ウィンドウハンドル
    /// </summary>
    IntPtr WindowHandle { get; }
}
