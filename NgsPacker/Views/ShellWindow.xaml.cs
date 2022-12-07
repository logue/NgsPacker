// -----------------------------------------------------------------------
// <copyright file="ShellWindow.xaml.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static NgsPacker.Views.ShellWindow;

namespace NgsPacker.Views
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        /// <summary>
        /// デスクトップ ウィンドウ マネージャーのP/in Voke
        /// </summary>
        /// <param name="hwnd">ハンドラ</param>
        /// <param name="dwAttribute">DWMWINDOWATTRIBUTE 列挙型の値として指定された、設定する値を示すフラグ。</param>
        /// <param name="pvAttribute">設定する属性値を含むオブジェクトへのポインター。</param>
        /// <param name="cbAttribute">pvAttribute パラメーターを使用して設定される属性値のサイズ</param>
        /// <returns>HRESULT値</returns>
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE attribute,
            ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
            uint cbAttribute);

#pragma warning disable SA1602 // Enumeration items should be documented
        /// <summary>
        /// DWMWINDOWATTRIBUTE 列挙型
        /// </summary>
        [Flags]
        public enum DWMWINDOWATTRIBUTE
        {

            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_MICA_EFFECT = 1029,
        }

        /// <summary>
        /// DWM_WINDOW_CORNER_PREFERENCE 列挙型
        /// </summary>
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3,
        }
#pragma warning restore SA1602 // Enumeration items should be documented

        /// <summary>
        /// Enable Mica on the given HWND.
        /// </summary>
        /// <param name="source">対象のハンドル</param>
        /// <param name="darkThemeEnabled">ダークモードか</param>
        public static void EnableMica(HwndSource source, bool darkThemeEnabled)
        {
            var trueValue = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
            var falseValue = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;

            // Set dark mode before applying the material, otherwise you'll get an ugly flash when displaying the window.
            if (darkThemeEnabled)
            {
                _ = DwmSetWindowAttribute(source.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, sizeof(uint));
            }
            else
            {
                _ = DwmSetWindowAttribute(source.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref falseValue, sizeof(uint));
            }

            _ = DwmSetWindowAttribute(source.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref trueValue, sizeof(uint));
        }

        /// <summary>
        /// スタイルを適用
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        public static void UpdateStyleAttributes(HwndSource hwnd)
        {
            // You can avoid using ModernWpf here and just rely on Win32 APIs or registry parsing if you want to.
            var darkThemeEnabled = ModernWpf.ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Dark;

            EnableMica(hwnd, darkThemeEnabled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindow"/> class.
        /// コンストラクタ
        /// </summary>
        public ShellWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            // Apply Mica brush and ImmersiveDarkMode if needed
            UpdateStyleAttributes((HwndSource)sender);

            // Hook to Windows theme change to reapply the brushes when needed
            ModernWpf.ThemeManager.Current.ActualApplicationThemeChanged += (s, ev) => UpdateStyleAttributes((HwndSource)sender);
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
