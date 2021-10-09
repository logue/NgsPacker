// -----------------------------------------------------------------------
// <copyright file="IZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using zamboni;

namespace NgsPacker.Interfaces
{
    public interface IZamboniService
    {
        /// <summary>
        /// 指定されたディレクトリをパックする
        /// </summary>
        /// <param name="inputPath">パックしたいファイルのディレクトリ</param>
        /// <param name="recursive">サブディレクトリを再帰的に含めるか</param>
        /// <param name="compress">圧縮する</param>
        /// <param name="forceUnencrypted">暗号化する（非推奨）</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public Task<byte[]> Pack(string inputPath, bool recursive = true, bool compress = false, bool forceUnencrypted = false);

        /// <summary>
        /// 指定されたファイルをアンパックする
        /// </summary>
        /// <param name="inputPath">入力ファイルのパス</param>
        /// <param name="subdir">サブディレクトリを作成する</param>
        /// <param name="sepalate">グループ1と2で分ける</param>
        public void Unpack(string inputPath, string outputPath = null, bool subdir = true, bool sepalate = false);

        /// <summary>
        /// ファイル一覧を取得
        /// </summary>
        /// <param name="inputPath">解析するdataディレクトリ</param>
        /// <returns></returns>
        public Task<List<string>> Filelist(string inputPath);

        /// <summary>
        /// Iceファイルを読み込む.
        /// </summary>
        /// <param name="inputPath">Iceファイルへのパス</param>
        /// <returns>Iceファイルのオブジェクト</returns>
        /// <exception cref="ZamboniException">パースできなかった場合</exception>
        public IceFile LoadIceFile(string inputPath);

        /// <summary>
        /// Iceファイルを読み込む（非同期版）.
        /// </summary>
        /// <param name="inputPath">Iceファイルへのパス</param>
        /// <returns>Iceファイルのオブジェクト</returns>
        /// <exception cref="ZamboniException">パースできなかった場合</exception>
        public Task<IceFile> LoadIceFileAsync(string inputPath);
    }
}
