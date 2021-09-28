// -----------------------------------------------------------------------
// <copyright file="ZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using zamboni;
using static zamboni.IceHeaderStructures;

namespace NgsPacker.Services
{
    /// <summary>
    ///  ZamboniLibのラッパー
    /// </summary>
    public class ZamboniService : IZamboniService
    {
        /// <summary>
        /// グループ１のホワイトリスト
        /// </summary>
        private List<string> WhiteList;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ZamboniService()
        {
            // 設定からホワイトリストを読み込む
            string[] whiteList = Properties.Settings.Default.WhiteList.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
            WhiteList = new List<string>(whiteList);
        }

        /// <summary>
        /// 指定されたディレクトリをパックする
        /// </summary>
        /// <param name="inputPath">パックしたいファイルのディレクトリ</param>
        /// <param name="recursive">サブディレクトリを再帰的に含めるか</param>
        /// <param name="compress">圧縮するか</param>
        /// <param name="forceUnencrypted">暗号化しない</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public byte[] Pack(string inputPath, bool recursive, bool compress, bool forceUnencrypted = false)
        {
            // ディレクトリの存在チェック
            if (!Directory.Exists(inputPath))
            {
                throw new DirectoryNotFoundException("ZamboniService: Input directory is not found.");
            }

            // ファイル一覧を取得
            string[] files = (recursive == true) ? Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories).ToArray() : Directory.GetFiles(inputPath);

            List<byte[]> group1Binaries = new();
            List<byte[]> group2Binaries = new();

            foreach (string currentFile in files)
            {
                string fileName = Path.GetFileName(currentFile);
                // ファイルを読み込む
                List<byte> file = new(File.ReadAllBytes(currentFile));

                // ヘッダを書き込む
                file.InsertRange(0, (new IceFileHeader(currentFile, (uint)file.Count)).GetBytes());

                // グループ1と2でファイルを振り分ける
                if (WhiteList.Contains(fileName))
                {
                    group1Binaries.Add(file.ToArray());
                }
                else
                {
                    group1Binaries.Add(file.ToArray());
                }
            }
            IceArchiveHeader header = new();
            IceV4File ice = new(header.GetBytes(), group1Binaries.ToArray(), group2Binaries.ToArray());
            return ice.getRawData(compress, forceUnencrypted);
        }

        /// <summary>
        /// 指定されたファイルをアンパックする
        /// </summary>
        /// <param name="inputPath">入力ファイルへのパス</param>
        /// <returns>解凍したファイルのバイナリ</returns>
        public byte[] Unpack(string inputPath)
        {
            MemoryStream ms = new();
            using FileStream fs = new(inputPath, FileMode.Open, FileAccess.Read);
            fs.CopyTo(ms);
            return IceFile.LoadIceFile(fs).header;
        }
    }
}
