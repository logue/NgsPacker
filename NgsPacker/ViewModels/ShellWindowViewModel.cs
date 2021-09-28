// -----------------------------------------------------------------------
// <copyright file="ShellWindowViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using ModernWpf.Controls;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace NgsPacker.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        public string Title { get; set; } = "Prism Application";

        public string PaneTitle { get; set; } = "Pane Title";

        /// <summary>
        /// リージョンマネージャー.
        /// </summary>
        private readonly IRegionManager RegionManager;

        /// <summary>
        /// ローディングリング
        /// </summary>
        public Visibility ProgressRing { get; set; } = Visibility.Hidden;

        /// <summary>
        /// ナビゲーション変更
        /// </summary>
        public DelegateCommand<NavigationViewSelectionChangedEventArgs> SelectionChangedCommand { get; private set; }

        /// <summary>
        /// アバウトボタンクリック時のコマンド.
        /// </summary>
        public DelegateCommand AboutCommand { get; set; }


        /// <summary>
        /// テーマ切り替えコマンド.
        /// </summary>
        public DelegateCommand ToggleThemeCommand { get; set; }

        /// <summary>
        /// 設定ボタンクリック時のコマンド.
        /// </summary>
        public DelegateCommand SettingCommand { get; set; }

        /// <summary>
        /// 終了コマンド.
        /// </summary>
        public DelegateCommand ExitCommand { get; set; }

        private Dictionary<string, Uri> Pages { get; set; } = new Dictionary<string, Uri>(){
            { "Home",  new Uri("HomePage", UriKind.Relative) },
            { "About",  new Uri("AboutPage", UriKind.Relative) },
            { "SettingsItem",  new Uri("SettingsPage", UriKind.Relative)}
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="regionManager">インジェクションするIRegionManager。.</param>
        public ShellWindowViewModel(IRegionManager regionManager)
        {
            Debug.WriteLine("イベント割当");
            RegionManager = regionManager;
            _ = regionManager.RegisterViewWithRegion("ContentRegion", typeof(HomePage));

            // ナビゲーション遷移
            SelectionChangedCommand = new DelegateCommand<NavigationViewSelectionChangedEventArgs>(SelectionChanged);

            // 終了
            ExitCommand = new DelegateCommand(ExecuteExitCommand);

            ToggleThemeCommand = new DelegateCommand(ToggleTheme);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindowViewModel"/> class.
        /// </summary>
        public ShellWindowViewModel()
        {
        }

        public void SelectionChanged(NavigationViewSelectionChangedEventArgs args)
        {
            try
            {
                NavigationViewItem selectedItem = (NavigationViewItem)args.SelectedItem;
                Debug.WriteLine("ナビゲーションあり", selectedItem.Name);
                // 対応するページ表示

                RegionManager.RequestNavigate("ContentRegion", Pages[selectedItem.Name]);
            }
            catch (Exception ex)
            {
                AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow, ex.Message);
            }
        }

        /// <summary>
        /// テーマ切り替え
        /// </summary>
        private void ToggleTheme()
        {
            if (ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Dark)
            {
                ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
                return;
            }

            ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;

        }

        /// <summary>
        /// 終了コマンド.
        /// </summary>
        public static void ExecuteExitCommand()
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// アクティブなウィンドウのハンドルを取得.
        /// </summary>
        /// <returns>.</returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        private static extern IntPtr GetActiveWindow();
    }
}
