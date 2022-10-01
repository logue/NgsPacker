// -----------------------------------------------------------------------
// <copyright file="ShellWindowViewModel.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ModernWpf;
using ModernWpf.Controls;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// 親画面のビューモデル.
    /// </summary>
    public class ShellWindowViewModel : BindableBase
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string Title { get; set; } = "Prism Application";

        /// <summary>
        /// リージョンマネージャー.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// 多言語化サービス
        /// </summary>
        private readonly ILocalizerService localizerService;

        /// <summary>
        /// 終了コマンド.
        /// </summary>
        public static void ExecuteExitCommand()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// ナビゲーション変更
        /// </summary>
        public DelegateCommand OnLoadedCommand { get; private set; }

        /// <summary>
        /// 読み込まれた
        /// </summary>
        public DelegateCommand<NavigationViewSelectionChangedEventArgs> SelectionChangedCommand { get; private set; }

        /// <summary>
        /// ページ名とビューの対応表
        /// </summary>
        private Dictionary<string, Uri> Pages { get; set; } = new Dictionary<string, Uri>()
        {
            { "Pack",  new Uri("PackPage", UriKind.Relative) },
            { "Unpack",  new Uri("UnpackPage", UriKind.Relative) },
            { "About",  new Uri("AboutPage", UriKind.Relative) },
            { "SettingsItem",  new Uri("SettingsPage", UriKind.Relative) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindowViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">インジェクションするIRegionManager。.</param>
        /// <param name="localizerService">多言語化サービス.</param>
        public ShellWindowViewModel(IRegionManager regionManager, ILocalizerService localizerService)
        {
            // アプリ名はアセンブリ名
            Title = AppAssemblyModel.Title;

            // ダークモード切替
            ThemeManager.Current.ApplicationTheme = Properties.Settings.Default.ThemeDark ?
                ApplicationTheme.Dark : ApplicationTheme.Light;

            // 初期状態のページ
            _ = regionManager.RegisterViewWithRegion("ContentRegion", typeof(UnpackPage));

            // 画面が読み込まれたときの処理
            OnLoadedCommand = new DelegateCommand(OnLoaded);

            // ナビゲーション遷移登録
            SelectionChangedCommand = new DelegateCommand<NavigationViewSelectionChangedEventArgs>(SelectionChanged);

            // リージョンマネージャーをインジェクション
            this.regionManager = regionManager;

            // 多言語サービスをインジェクション
            this.localizerService = localizerService;
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        public void OnLoaded()
        {
            if (!File.Exists(Properties.Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "pso2.exe"))
            {
                // pso.exe存在確認チェック
                _ = ModernWpf.MessageBox.Show(
                   this.localizerService.GetLocalizedString("Pso2ExeNotFoundErrorText"), this.localizerService.GetLocalizedString("ErrorTitleText"));

                // 設定ページに遷移
                this.regionManager.RequestNavigate("ContentRegion", Pages["SettingsItem"]);
            }
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
                regionManager.RequestNavigate("ContentRegion", Pages[selectedItem.Name]);
            }
            catch (Exception ex)
            {
                // _ = AcrylicMessageBox.Show(Application.Current.MainWindow, ex.Message, LocalizerService.GetLocalizedString("ErrorTitleText"));
                _ = ModernWpf.MessageBox.Show(ex.Message, localizerService.GetLocalizedString("ErrorTitleText"));
            }
        }
    }
}
