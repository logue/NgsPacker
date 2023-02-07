// -----------------------------------------------------------------------
// <copyright file="PackPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// パックページビューモデル
    /// </summary>
    public class PackPageViewModel : BindableBase
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
        /// ホワイトリスト
        /// </summary>
        public static string WhiteList
        {
            get => Properties.Settings.Default.WhiteList;
            set => Properties.Settings.Default.WhiteList = value;
        }

        /// <summary>
        /// パック
        /// </summary>
        public DelegateCommand PackCommand { get; private set; }

        /// <summary>
        /// 設定を保存
        /// </summary>
        public DelegateCommand SaveCommand { get; private set; }

        /// <summary>
        /// パック時に圧縮する
        /// </summary>
        public bool IsCompress { get; set; }

        /// <summary>
        /// パック時に暗号化する
        /// </summary>
        public bool IsCrypt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackPageViewModel"/> class.
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        public PackPageViewModel(ILocalizerService localizerService, IZamboniService zamboniService)
        {
            // パックのイベント割当
            PackCommand = new DelegateCommand(ExecutePackCommand);

            // 設定保存のイベント割当
            SaveCommand = new DelegateCommand(ExecuteSaveCommand);

            IsCompress = true;

            // サービスのインジェクション
            this.localizerService = localizerService;
            this.zamboniService = zamboniService;
        }

        /// <summary>
        /// パック処理
        /// </summary>
        private async void ExecutePackCommand()
        {
            // フォルダ選択ダイアログ
            FolderPicker picker = new();
            picker.Title = localizerService.GetLocalizedString("PackInputPathText");
            picker.InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            // ファイルダイアログを表示
            if (picker.ShowDialog() != true)
            {
                return;
            }

            // ファイル保存ダイアログ
            SaveFileDialog saveFileDialog = new()
            {
                Title = localizerService.GetLocalizedString("SaveAsDialogText"),
                Filter = localizerService.GetLocalizedString("IceFileFilterText"),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                FileName = "pso2data.ice",
            };

            // ダイアログを表示
            if (saveFileDialog.ShowDialog() != true)
            {
                // キャンセルされたので終了
                return;
            }

            // Iceで圧縮（結構重い）
            byte[] iceStream = await zamboniService.Pack(picker.ResultPath, IsCompress, IsCrypt);
            await File.WriteAllBytesAsync(saveFileDialog.FileName, iceStream);

            // 完了通知
            if (Properties.Settings.Default.NotifyComplete)
            {
                // トースト通知
                _ = new ToastContentBuilder()
                    .AddText(localizerService.GetLocalizedString("PackText"))
                    .AddText(localizerService.GetLocalizedString("CompleteText"));
            }
            else
            {
                _ = ModernWpf.MessageBox.Show(
                    localizerService.GetLocalizedString("PackText"), localizerService.GetLocalizedString("CompleteText"));
            }
        }

        /// <summary>
        /// 設定保存.
        /// </summary>
        private void ExecuteSaveCommand()
        {
            // 設定を保存
            Properties.Settings.Default.Save();
        }
    }
}