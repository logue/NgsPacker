// -----------------------------------------------------------------------
// <copyright file="SettingsPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Properties;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NgsPacker.ViewModels;

/// <summary>
///     設定ビューモデル
/// </summary>
public class SettingsPageViewModel : BindableBase
{
    /// <summary>
    ///     多言語化サービス.
    /// </summary>
    private readonly ILocalizeService localizeService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SettingsPageViewModel" /> class.
    /// </summary>
    /// <param name="localizeService">多言語化サービス.</param>
    public SettingsPageViewModel(ILocalizeService localizeService)
    {
        // ブラウズダイアログを表示するイベント割当
        BrowseCommand = new DelegateCommand(ExecuteBrowseCommand);

        // 設定保存のイベント割当
        SaveCommand = new DelegateCommand(ExecuteSaveCommand);

        // 多言語化サービスのインジェクション
        this.localizeService = localizeService;
    }

    /// <summary>
    ///     Pso2のバイナリディレクト
    /// </summary>
    public static string Pso2BinPath
    {
        get => Settings.Default.Pso2BinPath;
        set
        {
            Settings.Default.Pso2BinPath = value;
            Settings.Default.Save();
        }
    }

    /// <summary>
    ///     完了時に通知を出す.
    /// </summary>
    public static bool ToggleNotifyComplete
    {
        get => Settings.Default.NotifyComplete;
        set
        {
            Settings.Default.NotifyComplete = value;
            Settings.Default.Save();
        }
    }

    /// <summary>
    ///     ダークモード
    /// </summary>
    public static bool ToggleDarkTheme
    {
        get => Settings.Default.ThemeDark;
        set
        {
            ThemeManager.Current.ApplicationTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
            Settings.Default.ThemeDark = value;
            Settings.Default.Save();
        }
    }

    /// <summary>
    ///     pso2_binのディレクトリ選択
    /// </summary>
    public DelegateCommand BrowseCommand { get; }

    /// <summary>
    ///     対応言語.
    /// </summary>
    public IList<CultureInfo> SupportedLanguages => localizeService.SupportedLanguages;

    /// <summary>
    ///     選択されている言語.
    /// </summary>
    public CultureInfo SelectedLanguage
    {
        get => localizeService?.SelectedLanguage;
        set
        {
            if (localizeService != null && value != null && !value.Equals(localizeService.SelectedLanguage))
            {
                localizeService.SelectedLanguage = value;
                Settings.Default.Language = value.ToString();
                Settings.Default.Save();
            }
        }
    }

    /// <summary>
    ///     ホワイトリスト設定を保存
    /// </summary>
    public DelegateCommand SaveCommand { get; }

    /// <summary>
    ///     ホワイトリスト
    /// </summary>
    public static string WhiteList
    {
        get => Settings.Default.WhiteList;
        set => Settings.Default.WhiteList = value;
    }

    /// <summary>
    ///     pso2_binディレクトリ選択ダイアログを出す
    /// </summary>
    private void ExecuteBrowseCommand()
    {
        // フォルダ選択ダイアログ
        FolderPicker picker = new()
        {
            Title = localizeService.GetLocalizedString("SelectPso2BinPathText"), InputPath = Pso2BinPath
        };

        // 出力先ファイルダイアログを表示
        if (picker.ShowDialog() != true)
        {
            return;
        }

        if (!File.Exists(picker.ResultPath + Path.DirectorySeparatorChar + "pso2.exe"))
        {
            _ = MessageBox.Show(
                localizeService.GetLocalizedString("Pso2ExeNotFoundErrorText"),
                localizeService.GetLocalizedString("ErrorTitleText"));
            return;
        }

        Pso2BinPath = picker.ResultPath;
    }

    /// <summary>
    ///     ホワイトリストの設定保存.
    /// </summary>
    private void ExecuteSaveCommand()
    {
        // 設定を保存
        Settings.Default.Save();
    }
}
