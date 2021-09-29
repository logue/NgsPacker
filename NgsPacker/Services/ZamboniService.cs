// -----------------------------------------------------------------------
// <copyright file="ZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Exeptions;
using NgsPacker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly List<string> WhiteList;

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
        /// <param name="compress">圧縮する（非推奨）</param>
        /// <param name="forceUnencrypted">暗号化する（非推奨）</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public byte[] Pack(string inputPath, bool recursive = true, bool compress = false, bool forceUnencrypted = false)
        {
            // ディレクトリの存在チェック
            if (!Directory.Exists(inputPath))
            {
                throw new DirectoryNotFoundException("ZamboniService: Input directory is not found.");
            }

            // ファイル一覧を取得
            string[] files = recursive ? Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories).ToArray() : Directory.GetFiles(inputPath);

            // グループ1に書き込むバイナリデータ
            List<byte[]> group1Binaries = new();
            // グループ2に書き込むバイナリデータ
            List<byte[]> group2Binaries = new();

            // 入力ディレクトリ内のファイルを走査
            foreach (string currentFile in files)
            {
                // 入力ファイル
                string fileName = Path.GetFileName(currentFile);
                // ファイルをバイト配列として読み込む
                List<byte> file = new(File.ReadAllBytes(currentFile));

                // ヘッダを書き込む
                file.InsertRange(0, (new IceFileHeader(currentFile, (uint)file.Count)).GetBytes());

                // グループ1と2でファイルを振り分ける
                // 本プログラムでは設定画面から振り分けるファイルを定義する。
                //
                // ※PSO2のデータファイルは、拡張子やファイル名に応じてグループ1とグループ2で振り分けて書き込まれる。
                // 　たとえばテスクチャなどの.ddsファイルはグループ1に書き込まれるが、
                // 　モデルやポリゴンを定義するaqoファイルなどはグループ2に書き込まれないと動かない。
                // 　ちなみに、グループ1に書き込まれるoxyresource.crcは、Modを有効化させるとき必要だぞ。
                if (WhiteList.Contains(fileName))
                {
                    group1Binaries.Add(file.ToArray());
                }
                else
                {
                    group1Binaries.Add(file.ToArray());
                }
            }
            // ヘッダ
            IceArchiveHeader header = new();
            try
            {
                // Iceファイルとして書き出す
                IceV4File ice = new(header.GetBytes(), group1Binaries.ToArray(), group2Binaries.ToArray());
                return ice.getRawData(compress, forceUnencrypted);
            }
            catch (Exception e)
            {
                throw new ZamboniException("An error occurred while generating the Ice file.", e);
            }
        }

        /// <summary>
        /// 指定されたファイルをアンパックする
        /// </summary>
        /// <param name="inputPath">入力ファイルのパス</param>
        /// <param name="outputPath">出力先のパス</param>
        /// <param name="separateByGroup">グループ別に出力ディレクトリを分ける</param>
        /// <returns>解凍したファイルのバイナリ</returns>
        public void Unpack(string inputPath, string outputPath, bool separateByGroup = false)
        {
            // Iceファイルをバイトとして読み込む
            byte[] buffer = File.ReadAllBytes(inputPath);

            // IceFile.LoadIceFile(fs).header;

            // Iceファイルのヘッダチェック
            if (buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0)
            {
                throw new ZamboniException("Not ice file.");
            }
            using MemoryStream ms = new(buffer);

            // Iceファイルを読み込む
            IceFile iceFile = IceFile.LoadIceFile(ms);
            if (iceFile == null)
            {
                throw new ZamboniException("Could not parse ice file.");
            }

            // 入力ファイルのファイル名を取得
            string fileName = Path.GetFileName(inputPath);
            // 出力先のディレクトリ名
            string directoryName = Path.GetDirectoryName(inputPath);

            // 出力先のディレクトリ作成処理（repacker_ice.exeと同じ仕様）
            string directory = !directoryName.Equals(outputPath) ? Path.GetFileName(directoryName) + "_" : "";
            // ファイル名+_extというディレクトリに出力
            string exportPath = Path.Combine(outputPath, directory + Path.GetFileName(inputPath) + "_ext");
            // ディレクトリが存在しない場合作成する
            if (!Directory.Exists(exportPath))
            {
                _ = Directory.CreateDirectory(directory);
            }

            // グループ1のエクスポート
            bool ret1 = ExportByGroup(iceFile.groupOneFiles,
                separateByGroup ? Path.Combine(outputPath, Path.Combine(directory, "group1")) : exportPath);
            // グループ2のエクスポート
            bool ret2 = ExportByGroup(iceFile.groupTwoFiles,
                separateByGroup ? Path.Combine(outputPath, Path.Combine(directory, "group2")) : exportPath);

            // 出力できるファイルがない場合
            if (!ret1 && !ret2)
            {
                throw new ZamboniException($"Neither group1 nor group2 was dumped from {Path.GetFileName(inputPath)}.");
            }
        }

        /// <summary>
        /// Iceファイルの解凍処理
        /// </summary>
        /// <param name="data">グループ別に仕分けされたデータ</param>
        /// <param name="outputPath">出力先のパス</param>
        /// <returns></returns>
        private static bool ExportByGroup(byte[][] data, string outputPath)
        {
            if (!Directory.Exists(outputPath) && data != null && (uint)data.Length > 0U)
                Directory.CreateDirectory(outputPath);

            for (int index = 0; index < data.Length; ++index)
            {
                int int32 = BitConverter.ToInt32(data[index], 16);

                // ファイル名を取得
                string fileName = Encoding.ASCII.GetString(data[index], 64, int32).TrimEnd(new char[1]);

                // ヘッダーを取得
                int iceHeaderSize = BitConverter.ToInt32(data[index], 0xC);
                int newLength = data[index].Length - iceHeaderSize;

                // データを取得
                byte[] content = new byte[newLength];
                Array.ConstrainedCopy(data[index], iceHeaderSize, content, 0, newLength);

                // ファイルの中身バイト列に書き込む
                /*
                using BinaryWriter bw = new(new FileStream(fileName, FileMode.Create));
                bw.Write(content);
                */
                File.WriteAllBytes(outputPath + "\\" + fileName, content);

                data[index] = null;
            }

            return !(data == null || (uint)data.Length == 0);
        }
    }

}
