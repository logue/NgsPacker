// -----------------------------------------------------------------------
// <copyright file="ShellWindowViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using ModernWpf.Controls;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
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
        /// 多言語化サービス
        /// </summary>
        private readonly ILocalizerService LocalizerService;

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
        public ShellWindowViewModel(IRegionManager regionManager, ILocalizerService localizerService)
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
            // 多言語サービスをインジェクション
            LocalizerService = localizerService;
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
                _ = AcrylicMessageBox.Show(Application.Current.MainWindow, ex.Message, LocalizerService.GetLocalizedString("ErrorTitleText"));
            }
        }

        /// <summary>
        /// 終了コマンド.
        /// </summary>
        public static void ExecuteExitCommand()
        {
            Application.Current.Shutdown();
        }
    }
}
