// -----------------------------------------------------------------------
// <copyright file="SettingsPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ModernWpf;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// 設定ビューモデル
    /// </summary>
    public class SettingsPageViewModel : BindableBase
    {
        /// <summary>
        /// 多言語化サービス.
        /// </summary>
        private readonly ILocalizeService localizeService;

        /// <summary>
        /// Pso2のバイナリディレクト
        /// </summary>
        public static string Pso2BinPath
        {
            get => Properties.Settings.Default.Pso2BinPath;
            set
            {
                Properties.Settings.Default.Pso2BinPath = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// 完了時に通知を出す.
        /// </summary>
        public static bool ToggleNotifyComplete
        {
            get => Properties.Settings.Default.NotifyComplete;
            set
            {
                Properties.Settings.Default.NotifyComplete = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// ダークモード
        /// </summary>
        public static bool ToggleDarkTheme
        {
            get => Properties.Settings.Default.ThemeDark;
            set
            {
                ThemeManager.Current.ApplicationTheme = value ? ModernWpf.ApplicationTheme.Dark : ModernWpf.ApplicationTheme.Light;
                Properties.Settings.Default.ThemeDark = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// pso2_binのディレクトリ選択
        /// </summary>
        public DelegateCommand BrowseCommand { get; }

        /// <summary>
        /// 対応言語.
        /// </summary>
        public IList<CultureInfo> SupportedLanguages => localizeService.SupportedLanguages;

        /// <summary>
        /// 選択されている言語.
        /// </summary>
        public CultureInfo SelectedLanguage
        {
            get => localizeService?.SelectedLanguage;
            set
            {
                if (localizeService != null && value != null && !value.Equals(localizeService.SelectedLanguage))
                {
                    localizeService.SelectedLanguage = value;
                    Properties.Settings.Default.Language = value.ToString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="localizeService">多言語化サービス.</param>
        public SettingsPageViewModel(ILocalizeService localizeService)
        {
            // 設定保存のイベント割当
            BrowseCommand = new DelegateCommand(ExecuteBrowseCommand);

            // 多言語化サービスのインジェクション
            this.localizeService = localizeService;
        }

        /// <summary>
        /// pso2_binディレクトリのブラウズ
        /// </summary>
        private void ExecuteBrowseCommand()
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new ()
            {
                Title = localizeService.GetLocalizedString("SelectPso2BinPathText"),
                InputPath = Pso2BinPath,
            };

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(picker.ResultPath + Path.DirectorySeparatorChar + "pso2.exe"))
            {
                _ = ModernWpf.MessageBox.Show(
                   localizeService.GetLocalizedString("Pso2ExeNotFoundErrorText"), localizeService.GetLocalizedString("ErrorTitleText"));
                return;
            }

            Pso2BinPath = picker.ResultPath;
        }
    }
}
