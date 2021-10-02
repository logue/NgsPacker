// -----------------------------------------------------------------------
// <copyright file="FoldarPickerEx.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NgsPacker.Helpers
{
    /// <summary>
    /// UWP用のフォルダ選択ダイアログを無理やりWPFで使うクラス.
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
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.Desktop,
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
        /// The PickSingleFolder.
        /// </summary>
        /// <returns>The <see cref="StorageFolder"/>.</returns>
        public async Task<StorageFolder> PickSingleFolderAsync()
        {
            return await mfolderPicker.PickSingleFolderAsync();
        }

        /// <summary>
        /// The GetActiveWindow.
        /// </summary>
        /// <returns>The <see cref="IntPtr"/>.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
    }
}