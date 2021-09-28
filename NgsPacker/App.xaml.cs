// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Interfaces;
using NgsPacker.Services;
using NgsPacker.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace NgsPacker
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : PrismApplication
    {
        /// <summary>
        /// 外部プロセスのメイン・ウィンドウを起動するためのWin32 API.
        /// </summary>
        /// <param name="hwnd">The hWnd<see cref="IntPtr"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// The ShowWindowAsync.
        /// </summary>
        /// <param name="hWnd">The hWnd<see cref="IntPtr"/>.</param>
        /// <param name="nCmdShow">The nCmdShow<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 最小化状態か.
        /// </summary>
        /// <param name="hwnd">The hWnd<see cref="IntPtr"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hwnd);

        /// <summary>
        /// ShowWindowAsync関数のパラメータに渡す定義値(画面を元の大きさに戻す)..
        /// </summary>
        private const int SW_RESTORE = 9;

        /// <summary>
        /// The CreateShell.
        /// </summary>
        /// <returns>.</returns>
        protected override Window CreateShell()
        {
            // 複数インスタンスが動かないようにするための処理
            _ = new Semaphore(1, 1, Assembly.GetExecutingAssembly().GetName().Name, out bool createdNew);

            // まだアプリが起動してなければ
            if (createdNew)
            {
                return Container.Resolve<ShellWindow>();
            }

            // 既にアプリが起動していればそのアプリを前面に出す
            foreach (Process p in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                // 自分自身のプロセスIDは無視する
                if (p.Id != Environment.ProcessId)
                {
                    // プロセスのフルパス名を比較して同じアプリケーションか検証
                    if (p.MainModule.FileName == Process.GetCurrentProcess().MainModule.FileName)
                    {
                        // メイン・ウィンドウが最小化されていれば元に戻す
                        if (IsIconic(p.MainWindowHandle))
                        {
                            _ = ShowWindowAsync(p.MainWindowHandle, SW_RESTORE);
                        }

                        // メイン・ウィンドウを最前面に表示する
                        _ = SetForegroundWindow(p.MainWindowHandle);
                    }
                }
            }

            Current.Shutdown();
            return null;
        }

        /// <summary>
        /// コンテナを登録.
        /// </summary>
        /// <param name="containerRegistry">インジェクションするコンテナのレジストリ.</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 多言語化
            _ = containerRegistry.RegisterInstance<ILocalizerService>(Container.Resolve<LocalizerService>());
            // Zamboni
            _ = containerRegistry.RegisterInstance<IZamboniService>(Container.Resolve<ZamboniService>());

            // ページの登録
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<AboutPage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
        }
    }
}
