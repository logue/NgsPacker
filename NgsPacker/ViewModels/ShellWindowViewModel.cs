// -----------------------------------------------------------------------
// <copyright file="ShellWindowViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using ModernWpf.Controls;
using NgsPacker.Models;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace NgsPacker.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string Title { get; set; } = "Prism Application";

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
        /// ページ名とビューの対応表
        /// </summary>
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
            // アプリ名はアセンブリ名
            Title = AppAssemblyModel.Title;

            // ダークモード切替
            ThemeManager.Current.ApplicationTheme = Properties.Settings.Default.ThemeDark ? ModernWpf.ApplicationTheme.Dark : ModernWpf.ApplicationTheme.Light;

            // 初期状態のページ
            _ = regionManager.RegisterViewWithRegion("ContentRegion", typeof(HomePage));

            // ナビゲーション遷移登録j
            SelectionChangedCommand = new DelegateCommand<NavigationViewSelectionChangedEventArgs>(SelectionChanged);

            // リージョンマネージャーをインジェクション
            RegionManager = regionManager;
        }

        /// <summary>
        /// ナビゲーション
        /// </summary>
        /// <param name="args">選択された項目</param>
        public void SelectionChanged(NavigationViewSelectionChangedEventArgs args)
        {
            try
            {
                NavigationViewItem selectedItem = (NavigationViewItem)args.SelectedItem;
                // 対応するページ表示
                RegionManager.RequestNavigate("ContentRegion", Pages[selectedItem.Name]);
            }
            catch (Exception ex)
            {
                AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow, ex.Message);
            }
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
