// -----------------------------------------------------------------------
// <copyright file="ZamboniService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ImTools;
using NgsPacker.Exeptions;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                file.InsertRange(0, new IceFileHeader(currentFile, (uint)file.Count).GetBytes());

                // グループ1と2でファイルを振り分ける
                // 本プログラムでは設定画面から振り分けるファイルを定義する。
                //
                // ※PSO2のデータファイルは、拡張子やファイル名に応じてグループ1とグループ2で振り分けて書き込まれる。
                // 　たとえばテスクチャなどの.ddsファイルはグループ1に書き込まれるが、
                // 　モデルやポリゴンを定義するaqoファイルなどはグループ2に書き込まれないと動かない。
                // 　ちなみに、グループ1に書き込まれるoxyresource.crcは、Modを有効化させるとき必要だぞ。
                if (WhiteList.Contains(fileName))
                {
                    // グループ1に分類されるファイル
                    group1Binaries.Add(file.ToArray());
                }
                else
                {
                    // グループ2に分類されるファイル
                    group2Binaries.Add(file.ToArray());
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
        /// <param name="sepalate">グループ1と2で分ける</param>
        public void Unpack(string inputPath, string outputPath = null, bool sepalate = false)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Directory.GetCurrentDirectory();
            }
            // Iceファイルを読み込む
            IceFile iceFile = LoadIceFile(inputPath);

            // 出力先のディレクトリ（ファイル名_ext）　※repack_ice.exeと同じ仕様
            string destinaton = outputPath + Path.DirectorySeparatorChar + Path.GetFileName(inputPath) + "_ext" + Path.DirectorySeparatorChar;

            // グループ1
            List<IceEntryModel> groupOneFiles = IceToFilelist(iceFile.groupOneFiles, true);
            // グループ2
            List<IceEntryModel> groupTwoFiles = IceToFilelist(iceFile.groupTwoFiles);


            // 出力できるファイルがない場合
            if (groupOneFiles.Count == 0 && groupTwoFiles.Count == 0)
            {
                throw new ZamboniException($"Neither group1 nor group2 was dumped from {Path.GetFileName(inputPath)}.");
            }

            // 出力先のディレクトリ作成
            if (!Directory.Exists(destinaton))
            {
                _ = Directory.CreateDirectory(destinaton);
            }
            if (sepalate)
            {
                if (!Directory.Exists(destinaton + Path.DirectorySeparatorChar + "group1"))
                {
                    _ = Directory.CreateDirectory(destinaton + Path.DirectorySeparatorChar + "group1");
                }
                if (!Directory.Exists(destinaton + Path.DirectorySeparatorChar + "group2"))
                {
                    _ = Directory.CreateDirectory(destinaton + Path.DirectorySeparatorChar + "group2");
                }
            }

            foreach (IceEntryModel model in groupOneFiles.Concat(groupTwoFiles))
            {
                // ファイル名（ひどい可読性だ）
                string fileName = destinaton + (sepalate ?
                    (model.Group == IceGroupEnum.GROUP1 ? "group1" : "group2") + Path.DirectorySeparatorChar : "")
                    + model.FileName;

                using BinaryWriter writer = new(new FileStream(fileName, FileMode.Create));
                writer.Write(model.Content);
                writer.Close();
            }
        }

        /// <summary>
        /// ファイル一覧を取得
        /// </summary>
        /// <param name="inputPath">解析するdataディレクトリ</param>
        /// <returns></returns>
        public List<string> Filelist(string inputPath)
        {
            List<string> ret = new();
            string[] entries = Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories).ToArray();
            Debug.WriteLine("Entries: ", entries.Length);
            foreach (string path in entries)
            {
                if (path.Contains("."))
                {
                    continue;
                }
                Debug.WriteLine(path);
                // Iceファイルを読み込む
                using (FileStream fileStream = File.OpenRead(path))
                {
                    byte[] numArray = new byte[4];
                    _ = fileStream.Read(numArray, 0, 4);
                    _ = fileStream.Seek(0L, SeekOrigin.Begin);
                    

                    string str1 = Encoding.ASCII.GetString(numArray).Replace("\0", "");
                    if (str1 != "ICE")
                    {
                        fileStream.Close();
                        // ICEファイルでない
                        continue;
                    }
                    
                    ret.Add(Path.GetFileName(path) + "\t\t\t" + str1);

                    // Iceファイルをパースする
                    IceFile iceFile = IceFile.LoadIceFile(fileStream);

                    if (iceFile == null)
                    {
                        fileStream.Close();
                        // パース不能
                        continue;
                    }

                    // グループ1のファイルをパース
                    if (iceFile.groupOneFiles != null)
                    {
                        ret.Add("\tGroup 1:");
                        byte[][] groupOneFiles = iceFile.groupOneFiles;
                        for (int index = 0; index < groupOneFiles.Length; ++index)
                        {
                            int int32 = BitConverter.ToInt32(groupOneFiles[index], 16);
                            string str2 = Encoding.ASCII.GetString(groupOneFiles[index], 64, int32).TrimEnd(new char[1]);
                            ret.Add("\t\t" + str2);
                        }
                    }

                    // グループ2のファイルをパース
                    if (iceFile.groupOneFiles != null)
                    {
                        ret.Add("\tGroup 2:");
                        byte[][] groupTwoFiles = iceFile.groupTwoFiles;
                        for (int index = 0; index < groupTwoFiles.Length; ++index)
                        {
                            int int32 = BitConverter.ToInt32(groupTwoFiles[index], 16);
                            string str2 = Encoding.ASCII.GetString(groupTwoFiles[index], 64, int32).TrimEnd(new char[1]);
                            ret.Add("\t\t" + str2);
                        }
                    
                    }
                }
                ret.Add("");
                ret.Add("");
            }
            return ret;
        }

        /// <summary>
        /// Iceファイルを読み込む.
        /// </summary>
        /// <param name="inputPath">Iceファイルへのパス</param>
        /// <returns>Iceファイルのオブジェクト</returns>
        /// <exception cref="ZamboniException">パースできなかった場合</exception>
        public IceFile LoadIceFile(string inputPath)
        {
            // Iceファイルをバイトとして読み込む
            byte[] buffer = File.ReadAllBytes(inputPath);

            // IceFile.LoadIceFile(fs).header;

            // Iceファイルのヘッダチェック
            if (buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0)
            {
                throw new ZamboniException("Not ice file.");
            }
            // メモリーストリームを生成
            using MemoryStream ms = new(buffer);

            // Iceファイルを読み込む
            IceFile iceFile = IceFile.LoadIceFile(ms);
            if (iceFile == null)
            {
                throw new ZamboniException("Could not parse ice file.");
            }
            return iceFile;
        }

        /// <summary>
        /// Iceファイルを読み込む（非同期版）.
        /// </summary>
        /// <param name="inputPath">Iceファイルへのパス</param>
        /// <returns>Iceファイルのオブジェクト</returns>
        /// <exception cref="ZamboniException">パースできなかった場合</exception>
        public async Task<IceFile> LoadIceFileAsync(string inputPath)
        {
            // Iceファイルをバイトとして読み込む
            byte[] buffer = await File.ReadAllBytesAsync(inputPath);

            // Iceファイルのヘッダチェック
            if (buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0)
            {
                throw new ZamboniException("Not ice file.");
            }
            // メモリーストリームを生成
            using MemoryStream ms = new(buffer);

            // Iceファイルを読み込む
            IceFile iceFile = IceFile.LoadIceFile(ms);
            if (iceFile == null)
            {
                throw new ZamboniException("Could not parse ice file.");
            }
            return iceFile;
        }

        /// <summary>
        /// グループをファイルと内容の辞書型にする
        /// </summary>
        /// <param name="data">解凍済みのIceのデータストリーム</param>
        /// <returns></returns>
        public List<IceEntryModel> IceToFilelist(byte[][] data, bool isGroupOne = false)
        {
            List<IceEntryModel> fileList = new();
            for (int index = 0; index < data.Length; ++index)
            {
                int int32 = BitConverter.ToInt32(data[index], 16);

                // ヘッダーを取得
                int iceHeaderSize = BitConverter.ToInt32(data[index], 0xC);
                int newLength = data[index].Length - iceHeaderSize;

                // データを取得
                byte[] content = new byte[newLength];
                Array.ConstrainedCopy(data[index], iceHeaderSize, content, 0, newLength);

                // 配列に流し込む
                fileList.Add(new IceEntryModel()
                {
                    FileName = Encoding.ASCII.GetString(data[index], 64, int32).TrimEnd(new char[1]),
                    Content = content,
                    Group = isGroupOne ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP1
                });

                data[index] = null;
            }
            return fileList;
        }

        /// <summary>
        /// ファイルリストをIceEntryModelにする
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public List<IceEntryModel> FilelistToIce(string[] files)
        {
            List<IceEntryModel> ret = new();

            // 入力ディレクトリ内のファイルを走査
            foreach (string fileName in files)
            {
                // ファイルをバイト配列として読み込む
                ret.Add(new IceEntryModel()
                {
                    FileName = Path.GetFileName(fileName),
                    Content = File.ReadAllBytes(fileName),
                    Group = WhiteList.Contains(fileName) ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP2
                });
            }
            return ret;
        }

        /// <summary>
        /// ファイルリストをIceEntryModelにする
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<List<IceEntryModel>> FilelistToIceAsync(string[] files)
        {
            List<IceEntryModel> ret = new();

            // 入力ディレクトリ内のファイルを走査
            foreach (string fileName in files)
            {
                // ファイルをバイト配列として読み込む
                ret.Add(new IceEntryModel()
                {
                    FileName = Path.GetFileName(fileName),
                    Content = await File.ReadAllBytesAsync(fileName),
                    Group = WhiteList.Contains(fileName) ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP2
                });
            }
            return ret;
        }
    }
}