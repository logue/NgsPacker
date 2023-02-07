// -----------------------------------------------------------------------
// <copyright file="ToolsPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics;
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
        private readonly ILocalizerService localizerService;


        /// <summary>
        /// Zamboniサービス
        /// </summary>
        private readonly IZamboniService zamboniService;


        /// <summary>
        /// ファイルキャッシュDBサービス
        /// </summary>
        private readonly ICacheDbService cacheDbService;

        /// <summary>
        /// PSO2ディレクトリを対象
        /// </summary>
        public bool IsPso2 { get; set; }

        /// <summary>
        /// PSO2NGSディレクトリを対象
        /// </summary>
        public bool IsPso2Ngs { get; set; }


        /// <summary>
        /// スキャンボタンが押された
        /// </summary>
        public DelegateCommand<string> ScanCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsPageViewModel"/> class.
        /// コンストラクタ
        /// </summary>
        /// <param name="localizerService">多言語化サービス</param>
        /// <param name="zamboniService">Zamboniサービス</param>
        /// <param name="cacheDbService">ファイルキャッシュサービス</param>
        public ToolsPageViewModel(ILocalizerService localizerService, IZamboniService zamboniService, ICacheDbService cacheDbService)
        {
            // サービスのインジェクション
            this.localizerService = localizerService;
            this.zamboniService = zamboniService;
            this.cacheDbService = cacheDbService;

            ScanCommand = new DelegateCommand<string>(ExecuteScanCommand);
        }


        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="isFull">ファイルの中身も対象とする</param>
        private void ExecuteScanCommand(string isFull)
        {
            Debug.Print(isFull.ToString());
        }
    }
}
