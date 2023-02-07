// -----------------------------------------------------------------------
// <copyright file="ShellWindow.xaml.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using NgsPacker.Helpers;

namespace NgsPacker.Views
{
    /// <summary>EW
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow
    {
        /// <summary>
        /// スタイルを適用
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        public static void UpdateStyleAttributes(HwndSource hwnd)
        {
            // You can avoid using ModernWpf here and just rely on Win32 APIs or registry parsing if you want to.
            var darkThemeEnabled = ModernWpf.ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Dark;

            DwmApi.EnableMica(hwnd, darkThemeEnabled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindow"/> class.
        /// コンストラクタ
        /// </summary>
        public ShellWindow()
        {
            InitializeComponent();
            ContentRendered += Window_ContentRendered;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Get current hwnd
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);

            // Apply Mica brush and ImmersiveDarkMode if needed
            UpdateStyleAttributes(source);

            // Hook to Windows theme change to reapply the brushes when needed
            ModernWpf.ThemeManager.Current.ActualApplicationThemeChanged += (s, ev) => UpdateStyleAttributes(source);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get PresentationSource
            PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);

            // Subscribe to PresentationSource's ContentRendered event
            presentationSource.ContentRendered += Window_ContentRendered;
        }
    }
}
