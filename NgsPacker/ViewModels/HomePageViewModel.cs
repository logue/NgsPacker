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
        public bool ProgressDialog { get; private set; } = true;

        /// <summary>
        /// 進捗ダイアログのテキスト
        /// </summary>
        public string ProgressText { get; private set; }

        /// <summary>
        /// パック
        /// </summary>
        public DelegateCommand PackCommand { get; private set; }

        /// <summary>
        /// アンパック
        /// </summary>
        public DelegateCommand UnpackCommand { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        public HomePageViewModel(ILocalizerService localizerService, IZamboniService zamboniService)
        {
            PackCommand = new DelegateCommand(ExecutePackCommand);
            UnpackCommand = new DelegateCommand(ExecuteUnpackCommand);

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
            FolderPickerEx picker = new();

            // ファイルダイアログを表示
            Windows.Storage.StorageFolder folder = picker.PickSingleFolder();

            if (folder == null)
            {
                return;
            }

            // Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

            // ファイル保存ダイアログ
            using SaveFileDialog saveFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("SaveAsDialogText"),
                Filter = LocalizerService.GetLocalizedString("IceFileFilterText"),
                FileName = "pso2data.ice"
            };

            // ダイアログを表示
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // TODO:プログレスバーを表示
            ProgressDialog = true;
            ProgressText = LocalizerService.GetLocalizedString("PackingText");

            Task task = Task.Run(async () =>
            {
                // Iceで圧縮（結構重い）
                byte[] iceStream;
                try
                {
                    iceStream = ZamboniService.Pack(folder.Path, true, false);
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, iceStream);
                }
                catch (Exception ex)
                {
                    _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow, ex.Message, LocalizerService.GetLocalizedString("ErrorTitleText"));
                }
            });

            ProgressDialog = false;

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
        private void ExecuteUnpackCommand()
        {
            // ファイルを開くダイアログ
            using OpenFileDialog openFileDialog = new()
            {
                Title = LocalizerService.GetLocalizedString("UnpackDialogText"),
            };

            // ダイアログを表示
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                // キャンセルされたので終了
                return;
            }

            // フォルダ選択ダイアログ
            FolderPickerEx picker = new();

            // 出力先ファイルダイアログを表示
            Windows.Storage.StorageFolder folder = picker.PickSingleFolder();

            if (folder == null)
            {
                return;
            }
            // TODO:プログレスバーを表示
            ProgressDialog = true;
            ProgressText = LocalizerService.GetLocalizedString("UnpackingText");

            Task task = Task.Run(async () =>
            {
                try
                {
                    ZamboniService.Unpack(openFileDialog.FileName, folder.Path);
                }
                catch (Exception ex)
                {
                    _ = AcrylicMessageBox.Show(System.Windows.Application.Current.MainWindow, ex.Message, LocalizerService.GetLocalizedString("ErrorTitleText"));
                }
            });

            ProgressDialog = false;

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

    }
}
