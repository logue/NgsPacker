// -----------------------------------------------------------------------
// <copyright file="IZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

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
        /// <param name="compress">圧縮する（非推奨）</param>
        /// <param name="forceUnencrypted">暗号化する（非推奨）</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public byte[] Pack(string inputPath, bool recursive, bool compress = false, bool forceUnencrypted = false);

        /// <summary>
        /// 指定されたファイルをアンパックする
        /// </summary>
        /// <param name="inputPath">入力ファイルのパス</param>
        /// <param name="sepalate">グループ1と2で分ける</param>
        /// <returns>解凍したファイルのバイナリ</returns>
        public void Unpack(string inputPath, string outputPath = null, bool sepalate = false);

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
