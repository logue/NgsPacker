// -----------------------------------------------------------------------
// <copyright file="ShellWindow.xaml.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using NgsPacker.Helpers;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NgsPacker.Views;

/// <summary>
///     Interaction logic for ShellWindow.xaml
/// </summary>
public partial class ShellWindow
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ShellWindow" /> class.
    /// </summary>
    public ShellWindow()
    {
        InitializeComponent();
        ContentRendered += Window_ContentRendered;
    }

    /// <summary>
    ///     スタイルを適用
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    public static void UpdateStyleAttributes(HwndSource hWnd)
    {
        // You can avoid using ModernWpf here and just rely on Win32 APIs or registry parsing if you want to.
        bool darkThemeEnabled = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;

        DwmApi.EnableMica(hWnd, darkThemeEnabled);
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        // Get current hWnd
        HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);

        // Apply Mica brush and ImmersiveDarkMode if needed
        UpdateStyleAttributes(source);

        // Hook to Windows theme change to reapply the brushes when needed
        ThemeManager.Current.ActualApplicationThemeChanged += (s, ev) => UpdateStyleAttributes(source);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Get PresentationSource
        PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);

        // Subscribe to PresentationSource's ContentRendered event
        presentationSource.ContentRendered += Window_ContentRendered;
    }
}
