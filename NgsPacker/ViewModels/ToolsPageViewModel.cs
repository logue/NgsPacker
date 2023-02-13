// -----------------------------------------------------------------------
// <copyright file="ToolsPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// ツールページのViewModel
    /// </summary>
    public class ToolsPageViewModel : BindableBase
    {
        /// <summary>
        /// 多言語化サービス.
        /// </summary>
        private readonly ILocalizeService localizeService;


        /// <summary>
        /// Zamboniサービス
        /// </summary>
        private readonly IZamboniService zamboniService;


        /// <summary>
        /// ファイルキャッシュDBサービス
        /// </summary>
        private readonly ICacheDbService cacheDbService;

        /// <summary>
        /// 対象ディレクトリ
        /// </summary>
        public DataDirectoryType Target { get; set; } = DataDirectoryType.Ngs;

        /// <summary>
        /// スキャンボタンが押された
        /// </summary>
        public DelegateCommand ScanCommand { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsPageViewModel"/> class.
        /// コンストラクタ
        /// </summary>
        /// <param name="localizeService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        /// <param name="cacheDbService">ファイルキャッシュサービス</param>
        public ToolsPageViewModel(ILocalizeService localizeService, IZamboniService zamboniService, ICacheDbService cacheDbService)
        {
            // サービスのインジェクション
            this.localizeService = localizeService;
            this.zamboniService = zamboniService;
            this.cacheDbService = cacheDbService;

            ScanCommand = new DelegateCommand(ExecuteScanCommand);
        }


        /// <summary>
        /// スキャン処理
        /// </summary>
        private void ExecuteScanCommand()
        {
            Debug.Print(Target.ToString());
        }
    }
}
