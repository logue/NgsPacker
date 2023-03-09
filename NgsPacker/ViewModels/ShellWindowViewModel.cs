// -----------------------------------------------------------------------
// <copyright file="ShellWindowViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ModernWpf;
using ModernWpf.Controls;
using NgsPacker.Events;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using NgsPacker.Properties;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MessageBox = ModernWpf.MessageBox;

namespace NgsPacker.ViewModels;

/// <summary>
///     親画面のビューモデル.
/// </summary>
public class ShellWindowViewModel : BindableBase, IDisposable
{
    /// <summary>
    ///     イベントアグリエイター
    /// </summary>
    private readonly IEventAggregator eventAggregator;

    /// <summary>
    ///     多言語化サービス
    /// </summary>
    private readonly ILocalizeService localizeService;

    /// <summary>
    ///     進捗ダイアログ
    /// </summary>
    private readonly ProgressDialog progressDialog;

    /// <summary>
    ///     リージョンマネージャー.
    /// </summary>
    private readonly IRegionManager regionManager;

    private bool disposedValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ShellWindowViewModel" /> class.
    /// </summary>
    /// <param name="regionManager">インジェクションするIRegionManager。.</param>
    /// <param name="localizeService">多言語化サービス.</param>
    /// <param name="eventAggregator">イベントアグリエイター.</param>
    public ShellWindowViewModel(IRegionManager regionManager, ILocalizeService localizeService,
        IEventAggregator eventAggregator)
    {
        // アプリ名はアセンブリ名
        Title = AppAssemblyModel.Title;

        // ダークモード切替
        ThemeManager.Current.ApplicationTheme =
            Settings.Default.ThemeDark ? ApplicationTheme.Dark : ApplicationTheme.Light;

        // 初期状態のページ
        _ = regionManager.RegisterViewWithRegion("ContentRegion", typeof(UnpackPage));

        // 画面が読み込まれたときの処理
        OnLoadedCommand = new DelegateCommand(OnLoaded);

        // ナビゲーション遷移登録
        SelectionChangedCommand = new DelegateCommand<NavigationViewSelectionChangedEventArgs>(SelectionChanged);

        // リージョンマネージャーをインジェクション
        this.regionManager = regionManager;

        // 多言語サービスをインジェクション
        this.localizeService = localizeService;

        // 進捗ダイアログをセット
        progressDialog = new ProgressDialog
        {
            Title = Title, CancelMessage = localizeService.GetLocalizedString("CancelText")
        };

        eventAggregator
            .GetEvent<ProgressEvent>()
            .Subscribe(ProgressEventHandler);

        this.eventAggregator = eventAggregator;
    }

    /// <summary>
    ///     アプリケーション名
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     ナビゲーション変更
    /// </summary>
    public DelegateCommand OnLoadedCommand { get; }

    /// <summary>
    ///     読み込まれた
    /// </summary>
    public DelegateCommand<NavigationViewSelectionChangedEventArgs> SelectionChangedCommand { get; }

    /// <summary>
    ///     ページ名とビューの対応表
    /// </summary>
    private Dictionary<string, Uri> Pages { get; } = new()
    {
        { "Pack", new Uri("PackPage", UriKind.Relative) },
        { "Unpack", new Uri("UnpackPage", UriKind.Relative) },
        { "About", new Uri("AboutPage", UriKind.Relative) },
        { "Tools", new Uri("ToolsPage", UriKind.Relative) },
        { "SettingsItem", new Uri("SettingsPage", UriKind.Relative) }
    };

    // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    // ~ShellWindowViewModel()
    // {
    //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
    //     Dispose(disposing: false);
    // }

    /// <summary>
    ///     破棄処理
    /// </summary>
    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     終了コマンド.
    /// </summary>
    public static void ExecuteExitCommand()
    {
        Application.Current.Shutdown();
    }

    /// <summary>
    ///     画面が読み込まれたとき
    /// </summary>
    public void OnLoaded()
    {
        try
        {
            _ = IceUtility.GetDataDir();
        }
        catch (FileNotFoundException)
        {
            // pso.exe存在確認チェック
            _ = MessageBox.Show(
                localizeService.GetLocalizedString("Pso2ExeNotFoundErrorText"),
                localizeService.GetLocalizedString("ErrorTitleText"));

            // 設定ページに遷移
            regionManager.RequestNavigate("ContentRegion", Pages["SettingsItem"]);
        }
    }

    /// <summary>
    ///     ナビゲーション
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
            // _ = AcrylicMessageBox.Show(Application.Current.MainWindow, ex.Message, LocalizeService.GetLocalizedString("ErrorTitleText"));
            _ = MessageBox.Show(ex.Message, localizeService.GetLocalizedString("ErrorTitleText"));
        }
    }

    /// <summary>
    ///     破棄する
    /// </summary>
    /// <param name="disposing">破棄中か</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                foreach (IRegion region in regionManager.Regions)
                {
                    region.RemoveAll();
                }
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            // TODO: 大きなフィールドを null に設定します
            disposedValue = true;
        }
    }

    /// <summary>
    ///     進捗ダイアログのイベントハンドラ
    /// </summary>
    /// <param name="model">進捗イベントのモデル</param>
    private void ProgressEventHandler(ProgressEventModel model)
    {
        if (!model.IsVisible)
        {
            progressDialog.Close();
            return;
        }

        progressDialog.Show();
        progressDialog.Caption = model.Caption;
        progressDialog.Message = model.Message;
        progressDialog.Detail = model.Detail;
        progressDialog.Animation = model.Animation;
        progressDialog.Value = model.Value;
        progressDialog.Maximum = model.Maximum;

        if (progressDialog.HasUserCancelled)
        {
            // キャンセルボタンが押されたときは、キャンセル用のイベントバスにその旨を送る
            eventAggregator.GetEvent<ProgressCancelEvent>().Publish(true);
        }
    }
}
