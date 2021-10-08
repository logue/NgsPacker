// -----------------------------------------------------------------------
// <copyright file="HomePageViewModel.cs" company="Logue">
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NgsPacker.ViewModels
{
    public class HomePageViewModel : BindableBase
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
        /// 進捗ダイアログ表示
        /// </summary>
        public bool InProgress { get; private set; }

        /// <summary>
        /// 進捗ダイアログのテキスト
        /// </summary>
        public string ProgressText { get; private set; }

        /// <summary>
        /// アンパック
        /// </summary>
        public DelegateCommand UnpackCommand { get; private set; }

        /// <summary>
        /// アンパック時にグループによってディレクトリを分ける
        /// </summary>
        public bool IsSepareteByGroup { get; set; }

        /// <summary>
        /// パック
        /// </summary>
        public DelegateCommand PackCommand { get; private set; }

        /// <summary>
        /// パック時に圧縮する
        /// </summary>
        public bool IsCompress { get; set; }

        /// <summary>
        /// パック時に暗号化する
        /// </summary>
        public bool IsCrypt { get; set; }

        /// <summary>
        /// ファイル一覧を出色
        /// </summary>
        public DelegateCommand ExportFilelistCommand { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        public HomePageViewModel(ILocalizerService localizerService, IZamboniService zamboniService)
        {
            PackCommand = new DelegateCommand(ExecutePackCommand);
            UnpackCommand = new DelegateCommand(ExecuteUnpackCommand);
            ExportFilelistCommand = new DelegateCommand(ExecuteExportFilelistCommand);

            IsCompress = true;

            // サービスのインジェクション
            LocalizerService = localizerService;
            ZamboniService = zamboniService;
        }

        /// <summary>
        /// パック処理
        /// </summary>
        private async void ExecutePackCommand()
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.Title = LocalizerService.GetLocalizedString("PackInputPathText");
            picker.InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            // ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル保存ダイアログ
            using SaveFileDialog saveFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("SaveAsDialogText"),
                Filter = LocalizerService.GetLocalizedString("IceFileFilterText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                FileName = "pso2data.ice"
            };

            // ダイアログを表示
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            _ = await Task.Run(async () =>
            {
                // Iceで圧縮（結構重い）
                byte[] iceStream = await ZamboniService.Pack(picker.ResultPath, IsCompress, IsCrypt);
                await File.WriteAllBytesAsync(saveFileDialog.FileName, iceStream);

                return Task.CompletedTask;
            });

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                new ToastContentBuilder()
                    .AddText(LocalizerService.GetLocalizedString("PackText"))
                    .AddText(LocalizerService.GetLocalizedString("CompleteText"))
                    .Show();
            }
            else
            {
                _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow,
                    LocalizerService.GetLocalizedString("PackText"), LocalizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// アンパック処理
        /// </summary>
        private async void ExecuteUnpackCommand()
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

            _ = await Task.Run(async () =>
             {
                 await ZamboniService.Unpack(openFileDialog.FileName, picker.ResultPath, IsSepareteByGroup);
                 return Task.CompletedTask;
             });


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

            _ = await Task.Run(async () =>
              {
                  List<string> list = new(await ZamboniService.Filelist(picker.ResultPath));
                  await File.WriteAllTextAsync(saveFileDialog.FileName, string.Join("\r\n", list));
                  return Task.CompletedTask;
              });

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

        private void ExecuteUnpackByFileList()
        {
            // ファイルを開くダイアログ
            using OpenFileDialog openFileDialog = new()
            {
                Title = "Choose list file.",
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

            // 出力先ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }
        }
    }
}
