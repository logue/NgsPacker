// -----------------------------------------------------------------------
// <copyright file="UnpackPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using FastSearchLibrary;
using Microsoft.Toolkit.Uwp.Notifications;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Properties;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Windows.Forms;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = ModernWpf.MessageBox;

namespace NgsPacker.ViewModels;

/// <summary>
///     アンパックページビューモデル
/// </summary>
public class UnpackPageViewModel : BindableBase, INotifyPropertyChanged
{
    /// <summary>
    ///     多言語化サービス
    /// </summary>
    private readonly ILocalizeService localizeService;

    /// <summary>
    ///     Zamboniサービス
    /// </summary>
    private readonly IZamboniService zamboniService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnpackPageViewModel" /> class.
    /// </summary>
    /// <param name="localizeService">多言語化サービス</param>
    /// <param name="zamboniService">Zamboniサービス</param>
    public UnpackPageViewModel(ILocalizeService localizeService, IZamboniService zamboniService)
    {
        UnpackCommand = new DelegateCommand(ExecuteUnpackCommand);
        ExportFileListCommand = new DelegateCommand(ExecuteExportFileListCommand);
        UnpackByFileListCommand = new DelegateCommand(ExecuteUnpackByFileListCommand);

        // ドラッグアンドドロップハンドラ
        _ = PreviewDragOverCommand.Subscribe(OnPreviewDragOver).AddTo(Disposable);
        _ = DropCommand.Subscribe(OnDrop).AddTo(Disposable);

        // サービスのインジェクション
        this.localizeService = localizeService;
        this.zamboniService = zamboniService;
    }

    /// <summary>
    ///     アンパック
    /// </summary>
    public DelegateCommand UnpackCommand { get; }

    /// <summary>
    ///     アンパック時にグループによってディレクトリを分ける
    /// </summary>
    public bool IsSeparateByGroup { get; set; }

    /// <summary>
    ///     ファイル一覧を出力
    /// </summary>
    public DelegateCommand ExportFileListCommand { get; }

    /// <summary>
    ///     対象ディレクトリ
    /// </summary>
    public DataDirectoryType Target { get; set; } = DataDirectoryType.Ngs;

    /// <summary>
    ///     ファイル一覧を出力
    /// </summary>
    public DelegateCommand UnpackByFileListCommand { get; }

    /// <summary>
    ///     表示するイメージのファイル名.
    /// </summary>
    public ReactivePropertySlim<string> ViewImage { get; } = new();

    /// <summary>
    ///     PreviewDragOverイベントのコマンド.
    /// </summary>
    public ReactiveCommand<DragEventArgs> PreviewDragOverCommand { get; } = new();

    /// <summary>
    ///     Dropイベントのコマンド.
    /// </summary>
    public ReactiveCommand<DragEventArgs> DropCommand { get; } = new();

    /// <summary>
    ///     Disposeが必要な処理をまとめてやる.
    /// </summary>
    private CompositeDisposable Disposable { get; } = new();

    /// <summary>
    ///     アンパック処理
    /// </summary>
    private void ExecuteUnpackCommand()
    {
        // ファイルを開くダイアログ
        using OpenFileDialog openFileDialog = new()
        {
            Title = localizeService.GetLocalizedString("UnpackDialogText"),
            InitialDirectory = Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "data" +
                               Path.DirectorySeparatorChar
        };

        // ダイアログを表示
        DialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
        {
            // キャンセルされたので終了
            return;
        }

        // フォルダ選択ダイアログ
        FolderPicker picker = new()
        {
            Title = localizeService.GetLocalizedString("UnpackOutputPathText"),
            InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        };

        // 出力先ファイルダイアログを表示
        if (picker.ShowDialog() != true)
        {
            return;
        }

        // アンパック
        bool result = zamboniService.Unpack(openFileDialog.FileName, picker.ResultPath, true, IsSeparateByGroup);

        // 完了通知
        if (Settings.Default.NotifyComplete)
        {
            // トースト通知
            new ToastContentBuilder()
                .AddText(localizeService.GetLocalizedString("UnpackText"))
                .AddText(localizeService.GetLocalizedString(result ? "CompleteText" : "CancelledText"))
                .Show();
        }
        else
        {
            _ = MessageBox.Show(
                localizeService.GetLocalizedString(result ? "CompleteText" : "CancelledText"),
                localizeService.GetLocalizedString("UnpackText"));
        }
    }

    /// <summary>
    ///     ファイル一覧を出力
    /// </summary>
    private async void ExecuteExportFileListCommand()
    {
        // ファイル保存ダイアログ
        using SaveFileDialog saveFileDialog = new()
        {
            Title = localizeService.GetLocalizedString("SaveAsDialogText"),
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            FileName = Target + "_data.csv"
        };

        // ダイアログを表示
        DialogResult dialogResult = saveFileDialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
        {
            // キャンセルされたので終了
            return;
        }

        // ファイルリストを生成
        List<string> list = new(await zamboniService.FileList(Target));

        if (list.Count != 0)
        {
            // CSV出力
            await File.WriteAllTextAsync(saveFileDialog.FileName, string.Join("\r\n", list));
        }

        // 完了通知
        if (Settings.Default.NotifyComplete)
        {
            // トースト通知
            new ToastContentBuilder()
                .AddText(localizeService.GetLocalizedString("ExportFileListText"))
                .AddText(localizeService.GetLocalizedString("CompleteText"))
                .Show();
        }
        else
        {
            _ = MessageBox.ShowAsync(
                localizeService.GetLocalizedString("CompleteText"),
                localizeService.GetLocalizedString("ExportFileListText"));
        }
    }

    /// <summary>
    ///     ファイルリストからアンパックする
    /// </summary>
    private void ExecuteUnpackByFileListCommand()
    {
        // ファイルを開くダイアログ
        using OpenFileDialog openFileDialog = new()
        {
            Title = localizeService.GetLocalizedString("UnpackByFileListDialogText"),
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        };

        // ダイアログを表示
        DialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
        {
            // キャンセルされたので終了
            return;
        }

        List<string> fileList = new(File.ReadAllLines(openFileDialog.FileName));

        // 出力先ディレクトリ
        string outputPath = Path.GetDirectoryName(openFileDialog.FileName) + Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(openFileDialog.FileName);

        if (!Directory.Exists(outputPath))
        {
            _ = Directory.CreateDirectory(outputPath);
        }

        fileList.ForEach(file =>
        {
            string path = IceUtility.GetDataDir() + file;
            Debug.WriteLine(path, outputPath);

            if (File.Exists(path))
            {
                // アンパック
                zamboniService.Unpack(path, outputPath, false);
            }
        });

        // 完了通知
        if (Settings.Default.NotifyComplete)
        {
            // トースト通知
            new ToastContentBuilder()
                .AddText(localizeService.GetLocalizedString("UnpackByFileListText"))
                .AddText(localizeService.GetLocalizedString("CompleteText"))
                .Show();
        }
        else
        {
            _ = MessageBox.ShowAsync(
                localizeService.GetLocalizedString("CompleteText"),
                localizeService.GetLocalizedString("UnpackByFileListText"));
        }
    }

    /// <summary>
    ///     ImageのPreviewDragOverイベントに対する処理.
    /// </summary>
    /// <param name="e">.</param>
    private void OnPreviewDragOver(DragEventArgs e)
    {
        // マウスカーソルをコピーにする。
        e.Effects = DragDropEffects.Copy;

        // ドラッグされてきたものがFileDrop形式の場合だけ、このイベントを処理済みにする。
        e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
    }

    /// <summary>
    ///     ImageのDropイベントに対する処理.
    /// </summary>
    /// <param name="e">.</param>
    private void OnDrop(DragEventArgs e)
    {
        // フォルダ選択ダイアログ
        FolderPicker picker = new()
        {
            Title = localizeService.GetLocalizedString("UnpackDirectoryDialogText"),
            InputPath = Settings.Default.Pso2BinPath
        };

        // 出力先ファイルダイアログを表示
        if (picker.ShowDialog() != true)
        {
            return;
        }

        // ドロップされたものがFileDrop形式の場合は、各ファイルのパス文字列を文字列配列に格納する。
        List<FileInfo> fileList = FileSearcher.GetFilesFast((string)e.Data.GetData(DataFormats.FileDrop), "*.*");

        // 出力先ディレクトリ
        string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if (!Directory.Exists(outputPath))
        {
            _ = Directory.CreateDirectory(outputPath);
        }

        fileList.ForEach(file =>
        {
            string path = picker.ResultPath + Path.DirectorySeparatorChar + file.Name;
            if (File.Exists(path))
            {
                // アンパック
                zamboniService.Unpack(path, outputPath, false);
            }
        });

        // 完了通知
        if (Settings.Default.NotifyComplete)
        {
            // トースト通知
            new ToastContentBuilder()
                .AddText(localizeService.GetLocalizedString("UnpackByFileListText"))
                .AddText(localizeService.GetLocalizedString("CompleteText"))
                .Show();
        }
        else
        {
            _ = MessageBox.Show(
                localizeService.GetLocalizedString("CompleteText"),
                localizeService.GetLocalizedString("UnpackByFileListText"));
        }
    }
}
