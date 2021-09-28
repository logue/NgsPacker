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
using System.Diagnostics;
using System.IO;
using System.Windows;
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
        public Visibility ProgressDialog { get; private set; } = Visibility.Collapsed;

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

            ProgressDialog = Visibility.Visible;
            ProgressText = LocalizerService.GetLocalizedString("PackingText");

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

            ProgressDialog = Visibility.Collapsed;

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
            Debug.WriteLine("アンパック");
            using OpenFileDialog fileOpenDialog = new();

            // ファイル選択ダイアログを表示
            DialogResult dialogResult = fileOpenDialog.ShowDialog();
            if (dialogResult == DialogResult.Cancel)
            {
                // キャンセルされたので終了
                return;
            }
            // フォルダ選択ダイアログ
            FolderPickerEx picker = new();

            // ファイルダイアログを表示
            Windows.Storage.StorageFolder folder = picker.PickSingleFolder();

            if (folder == null)
            {
                return;
            }

            // TODO:
        }

    }
}
