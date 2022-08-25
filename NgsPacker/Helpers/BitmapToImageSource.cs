// -----------------------------------------------------------------------
// <copyright file="BitmapToImageSource.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NgsPacker.Helper
{
    /// <summary>
    /// ビットマップをImageSourceにするクラス.
    /// </summary>
    public class BitmapToImageSource
    {
        /// <summary>
        /// The DeleteObject.
        /// </summary>
        /// <param name="hObject">The hObject<see cref="IntPtr"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// BitmapをImageSourceに変換する処理.
        /// </summary>
        /// <param name="bmp">.</param>
        /// <returns>.</returns>
        public static ImageSource Convert(Bitmap bmp)
        {
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                _ = DeleteObject(handle);
            }
        }
    }
}