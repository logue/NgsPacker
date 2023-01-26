// -----------------------------------------------------------------------
// <copyright file="UnpackPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Views;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// アンパックページビューモデル
    /// </summary>
    public class UnpackPageViewModel : BindableBase, INotifyPropertyChanged
    {
        /// <summary>
        /// 多言語化サービス
        /// </summary>
        private readonly ILocalizerService localizerService;

        /// <summary>
        /// Zamboniサービス
        /// </summary>
        private readonly IZamboniService zamboniService;

        /// <summary>
        /// アンパック
        /// </summary>
        public DelegateCommand UnpackCommand { get; private set; }

        /// <summary>
        /// アンパック時にグループによってディレクトリを分ける
        /// </summary>
        public bool IsSepareteByGroup { get; set; }

        /// <summary>
        /// ファイル一覧を出力
        /// </summary>
        public DelegateCommand ExportFilelistCommand { get; private set; }

        /// <summary>
        /// ファイル一覧を出力
        /// </summary>
        public DelegateCommand UnpackByFilelistCommand { get; private set; }

        /// <summary>
        /// 表示するイメージのファイル名.
        /// </summary>
        public ReactivePropertySlim<string> ViewImage { get; } = new ReactivePropertySlim<string>();

        /// <summary>
        /// PreviewDragOverイベントのコマンド.
        /// </summary>
        public ReactiveCommand<DragEventArgs> PreviewDragOverCommand { get; } = new ReactiveCommand<DragEventArgs>();

        /// <summary>
        /// Dropイベントのコマンド.
        /// </summary>
        public ReactiveCommand<DragEventArgs> DropCommand { get; } = new ReactiveCommand<DragEventArgs>();

        /// <summary>
        /// Disposeが必要な処理をまとめてやる.
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpackPageViewModel"/> class.
        /// コンストラクタ
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        public UnpackPageViewModel(ILocalizerService localizerService, IZamboniService zamboniService)
        {
            UnpackCommand = new DelegateCommand(ExecuteUnpackCommand);
            ExportFilelistCommand = new DelegateCommand(ExecuteExportFilelistCommand);
            UnpackByFilelistCommand = new DelegateCommand(ExecuteUnpackByFilelistCommand);

            // ドラッグアンドドロップハンドラ
            _ = PreviewDragOverCommand.Subscribe(OnPreviewDragOver).AddTo(Disposable);
            _ = DropCommand.Subscribe(OnDrop).AddTo(Disposable);

            // サービスのインジェクション
            this.localizerService = localizerService;
            this.zamboniService = zamboniService;
        }

        /// <summary>
        /// アンパック処理
        /// </summary>
        private void ExecuteUnpackCommand()
        {
            // ファイルを開くダイアログ
            using System.Windows.Forms.OpenFileDialog openFileDialog = new ()
            {
                Title = localizerService.GetLocalizedString("UnpackDialogText"),
                InitialDirectory = Properties.Settings.Default.Pso2BinPath,
            };

            // ダイアログを表示
            System.Windows.Forms.DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != System.Windows.Forms.DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // フォルダ選択ダイアログ
            FolderPicker picker = new ();
            picker.Title = localizerService.GetLocalizedString("UnpackOutputPathText");
            picker.InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // アンパック
            zamboniService.Unpack(openFileDialog.FileName, picker.ResultPath, true, IsSepareteByGroup);

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(localizerService.GetLocalizedString("UnpackText"))
                    .AddText(localizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = ModernWpf.MessageBox.Show(
                    localizerService.GetLocalizedString("UnpackText"), localizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// ファイル一覧を出力
        /// </summary>
        private async void ExecuteExportFilelistCommand()
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new ();
            picker.InputPath = Properties.Settings.Default.Pso2BinPath;

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル保存ダイアログ
            using System.Windows.Forms.SaveFileDialog saveFileDialog = new ()
            {
                Title = localizerService.GetLocalizedString("SaveAsDialogText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                FileName = "list.csv",
            };

            // ダイアログを表示
            System.Windows.Forms.DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != System.Windows.Forms.DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // 出力処理
            List<string> list = new (await zamboniService.Filelist(picker.ResultPath));
            await File.WriteAllTextAsync(saveFileDialog.FileName, string.Join("\r\n", list));

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(localizerService.GetLocalizedString("ExportFileListText"))
                    .AddText(localizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = ModernWpf.MessageBox.Show(
                    localizerService.GetLocalizedString("ExportFileListText"), localizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// ファイルリストからアンパックする
        /// </summary>
        private async void ExecuteUnpackByFilelistCommand()
        {
            // ファイルを開くダイアログ
            using System.Windows.Forms.OpenFileDialog openFileDialog = new ()
            {
                Title = localizerService.GetLocalizedString("UnpackByFileListDialogText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            };

            // ダイアログを表示
            System.Windows.Forms.DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != System.Windows.Forms.DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // フォルダ選択ダイアログ
            FolderPicker picker = new ();
            picker.Title = localizerService.GetLocalizedString("UnpackDirectoryDialogText");
            picker.InputPath = Properties.Settings.Default.Pso2BinPath;

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル一覧を読み込む
            List<string> fileList = new (await File.ReadAllLinesAsync(openFileDialog.FileName));

            // 出力先ディレクトリ
            string outputPath = Path.GetDirectoryName(openFileDialog.FileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(openFileDialog.FileName);

            if (!Directory.Exists(outputPath))
            {
                _ = Directory.CreateDirectory(outputPath);
            }

            ProgressDialog progressDialog = new ();
            _ = progressDialog.ShowAsync();
            foreach (string file in fileList)
            {
                string path = picker.ResultPath + Path.DirectorySeparatorChar + file;
                if (!File.Exists(path))
                {
                    // ファイルが存在しないときスキップ
                    continue;
                }

                // アンパック
                zamboniService.Unpack(path, outputPath, false);
            }

            progressDialog.Hide();

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(localizerService.GetLocalizedString("UnpackByFileListText"))
                    .AddText(localizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = ModernWpf.MessageBox.Show(
                    localizerService.GetLocalizedString("UnpackByFileListText"), localizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// ImageのPreviewDragOverイベントに対する処理.
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
        /// ImageのDropイベントに対する処理.
        /// </summary>
        /// <param name="e">.</param>
        private void OnDrop(DragEventArgs e)
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.Title = localizerService.GetLocalizedString("UnpackDirectoryDialogText");
            picker.InputPath = Properties.Settings.Default.Pso2BinPath;

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ドロップされたものがFileDrop形式の場合は、各ファイルのパス文字列を文字列配列に格納する。
            List<string> fileList = new((string[])e.Data.GetData(DataFormats.FileDrop));

            // 出力先ディレクトリ
            string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(outputPath))
            {
                _ = Directory.CreateDirectory(outputPath);
            }

            ProgressDialog progressDialog = new();
            _ = progressDialog.ShowAsync();
            foreach (string file in fileList)
            {
                string path = picker.ResultPath + Path.DirectorySeparatorChar + file;
                if (!File.Exists(path))
                {
                    // ファイルが存在しないときスキップ
                    continue;
                }

                // アンパック
                zamboniService.Unpack(path, outputPath, false);
            }

            progressDialog.Hide();

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(localizerService.GetLocalizedString("UnpackByFileListText"))
                    .AddText(localizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = ModernWpf.MessageBox.Show(
                    localizerService.GetLocalizedString("UnpackByFileListText"), localizerService.GetLocalizedString("CompleteText"));
            }
        }
    }
}
