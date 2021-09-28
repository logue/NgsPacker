// -----------------------------------------------------------------------
// <copyright file="FoldarPickerEx.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NgsPacker.Helpers
{
    /// <summary>
    /// Defines the <see cref="FolderPickerEx" />.
    /// </summary>
    public class FolderPickerEx
    {
        /// <summary>
        /// Defines the mfolderPicker.
        /// </summary>
        private readonly FolderPicker mfolderPicker;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPickerEx"/> class.
        /// </summary>
        public FolderPickerEx()
        {
            mfolderPicker = new FolderPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };
            mfolderPicker.FileTypeFilter.Add("*");
            IntPtr hwnd = GetActiveWindow();
            WinRT.Interop.InitializeWithWindow.Initialize(mfolderPicker, hwnd);
        }

        /// <summary>
        /// The PickSingleFolder.
        /// </summary>
        /// <returns>The <see cref="StorageFolder"/>.</returns>
        public StorageFolder PickSingleFolder()
        {
            return mfolderPicker.PickSingleFolderAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// The GetActiveWindow.
        /// </summary>
        /// <returns>The <see cref="IntPtr"/>.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
    }
}