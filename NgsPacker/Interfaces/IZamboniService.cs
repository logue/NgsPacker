// -----------------------------------------------------------------------
// <copyright file="IZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace NgsPacker.Interfaces
{
    public interface IZamboniService
    {
        /// <summary>
        /// 指定されたディレクトリをパックする
        /// </summary>
        /// <param name="inputPath">パックしたいファイルのディレクトリ</param>
        /// <param name="recursive">サブディレクトリを再帰的に含めるか</param>
        /// <param name="compress">圧縮するか</param>
        /// <param name="forceUnencrypted">暗号化しない</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public byte[] Pack(string inputPath, bool recursive, bool compress, bool forceUnencrypted = false);

        /// <summary>
        /// 指定されたファイルをアンパックする
        /// </summary>
        /// <param name="inputPath">入力ファイルへのパス</param>
        /// <returns>解凍したファイルのバイナリ</returns>
        public byte[] Unpack(string inputPath);
    }
}
