// -----------------------------------------------------------------------
// <copyright file="PackPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using ModernWpf;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Properties;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;

namespace NgsPacker.ViewModels;

/// <summary>
///     パックページビューモデル
/// </summary>
public class PackPageViewModel : BindableBase
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
    ///     Initializes a new instance of the <see cref="PackPageViewModel" /> class.
    /// </summary>
    /// <param name="localizeService">多言語化サービス</param>
    /// <param name="zamboniService">Zamboniサービス</param>
    public PackPageViewModel(ILocalizeService localizeService, IZamboniService zamboniService)
    {
        // パックのイベント割当
        PackCommand = new DelegateCommand(ExecutePackCommand);
        IsCompress = true;

        // サービスのインジェクション
        this.localizeService = localizeService;
        this.zamboniService = zamboniService;
    }

    /// <summary>
    ///     パック
    /// </summary>
    public DelegateCommand PackCommand { get; }

    /// <summary>
    ///     パック時に圧縮する
    /// </summary>
    public bool IsCompress { get; set; }

    /// <summary>
    ///     パック時に暗号化する
    /// </summary>
    public bool IsCrypt { get; set; }

    /// <summary>
    ///     パック処理
    /// </summary>
    private async void ExecutePackCommand()
    {
        // フォルダ選択ダイアログ
        FolderPicker picker = new()
        {
            Title = localizeService.GetLocalizedString("PackInputPathText"),
            InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        };

        // ファイルダイアログを表示
        if (picker.ShowDialog() != true)
        {
            return;
        }

        // ファイル保存ダイアログ
        SaveFileDialog saveFileDialog = new()
        {
            Title = localizeService.GetLocalizedString("SaveAsDialogText"),
            Filter = localizeService.GetLocalizedString("IceFileFilterText"),
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            FileName = "pso2data.ice"
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
        if (Settings.Default.NotifyComplete)
        {
            // トースト通知
            _ = new ToastContentBuilder()
                .AddText(localizeService.GetLocalizedString("PackText"))
                .AddText(localizeService.GetLocalizedString("CompleteText"));
        }
        else
        {
            _ = MessageBox.Show(
                localizeService.GetLocalizedString("CompleteText"), localizeService.GetLocalizedString("PackText"));
        }
    }
}
