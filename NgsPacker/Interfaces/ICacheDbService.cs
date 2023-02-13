// -----------------------------------------------------------------------
// <copyright file="ICacheDbService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using NgsPacker.Helpers;

namespace NgsPacker.Interfaces
{
    /// <summary>
    /// ファイルキャッシュサービスインターフェース
    /// </summary>
    public interface ICacheDbService
    {
        /// <summary>
        /// 対象ディレクトリ内のファイルとハッシュをDBに保存する
        /// </summary>
        /// <param name="target">対象</param>
        /// <param name="force">すべて書き直し</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ScanFileｓ(DataDirectoryType target = DataDirectoryType.Ngs, bool force = false);

        /// <summary>
        /// ファイルの内容物レコードを作成する
        /// </summary>
        /// <param name="target">対象</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ScanContents(DataDirectoryType target = DataDirectoryType.All);
    }
}
