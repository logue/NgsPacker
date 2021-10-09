// -----------------------------------------------------------------------
// <copyright file="UnpackPageViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Toolkit.Uwp.Notifications;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// アンパックページビューモデル
    /// </summary>
    public class UnpackPageViewModel : BindableBase
    {
        /// <summary>
        /// 多言語化サービス
        /// </summary>
        private readonly ILocalizerService LocalizerService;

        /// <summary>
        /// Zamboniサービス
        /// </summary>
        private readonly IZamboniService ZamboniService;

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
        /// コンストラクタ
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        public UnpackPageViewModel(ILocalizerService localizerService, IZamboniService zamboniService)
        {
            UnpackCommand = new DelegateCommand(ExecuteUnpackCommand);
            ExportFilelistCommand = new DelegateCommand(ExecuteExportFilelistCommand);
            UnpackByFilelistCommand = new DelegateCommand(ExecuteUnpackByFilelistCommand);

            // サービスのインジェクション
            LocalizerService = localizerService;
            ZamboniService = zamboniService;
        }

        /// <summary>
        /// アンパック処理
        /// </summary>
        private void ExecuteUnpackCommand()
        {
            // ファイルを開くダイアログ
            using OpenFileDialog openFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("UnpackDialogText"),
                InitialDirectory = Properties.Settings.Default.Pso2BinPath
            };

            // ダイアログを表示
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.Title = LocalizerService.GetLocalizedString("UnpackOutputPathText");
            picker.InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // アンパック
            ZamboniService.Unpack(openFileDialog.FileName, picker.ResultPath, true, IsSepareteByGroup);

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(LocalizerService.GetLocalizedString("UnpackText"))
                    .AddText(LocalizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow,
                    LocalizerService.GetLocalizedString("UnpackText"), LocalizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// ファイル一覧を出力
        /// </summary>
        private async void ExecuteExportFilelistCommand()
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.InputPath = Properties.Settings.Default.Pso2BinPath;

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル保存ダイアログ
            using SaveFileDialog saveFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("SaveAsDialogText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                FileName = "list.csv"
            };

            // ダイアログを表示
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // 出力処理
            List<string> list = new(await ZamboniService.Filelist(picker.ResultPath));
            await File.WriteAllTextAsync(saveFileDialog.FileName, string.Join("\r\n", list));

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(LocalizerService.GetLocalizedString("ExportFileListText"))
                    .AddText(LocalizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow,
                    LocalizerService.GetLocalizedString("ExportFileListText"), LocalizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// ファイルリストからアンパックする
        /// </summary>
        private async void ExecuteUnpackByFilelistCommand()
        {
            // ファイルを開くダイアログ
            using OpenFileDialog openFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("UnpackByFileListDialogText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            };

            // ダイアログを表示
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.Title = LocalizerService.GetLocalizedString("UnpackDirectoryDialogText");
            picker.InputPath = Properties.Settings.Default.Pso2BinPath;

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル一覧を読み込む
            List<string> fileList = new(await File.ReadAllLinesAsync(openFileDialog.FileName));

            // 出力先ディレクトリ
            string outputPath = Path.GetDirectoryName(openFileDialog.FileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(openFileDialog.FileName);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (string file in fileList)
            {
                string path = picker.ResultPath + Path.DirectorySeparatorChar + file;
                if (!File.Exists(path))
                {
                    // ファイルが存在しないときスキップ
                    continue;
                }
                // アンパック
                ZamboniService.Unpack(path, outputPath, false);
            }

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(LocalizerService.GetLocalizedString("UnpackByFileListText"))
                    .AddText(LocalizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow,
                    LocalizerService.GetLocalizedString("UnpackByFileListText"), LocalizerService.GetLocalizedString("CompleteText"));
            }
        }
    }
}
