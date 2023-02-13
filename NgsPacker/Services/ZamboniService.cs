// -----------------------------------------------------------------------
// <copyright file="ZamboniService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImTools;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using Prism.Events;
using Zamboni;
using Zamboni.IceFileFormats;
using static Zamboni.IceFileFormats.IceHeaderStructures;

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
        private readonly List<string> whiteList;

        /// <summary>
        /// 多言語化サービス
        /// </summary>
        private readonly ILocalizeService localizeService;

        /// <summary>
        /// 進捗ダイアログ.
        /// </summary>
        private readonly Views.ProgressDialog progressDialog;

        /// <summary>
        /// イベントアグリエイター
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Iceファイルヘッダー簡易チェック
        /// </summary>
        /// <param name="buffer">データのバイト</param>
        /// <returns>Iceファイルである場合false、Ice出ない場合true</returns>
        public static bool IsNotIce(byte[] buffer)
        {
            return buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0;
        }

        /// <summary>
        /// グループをファイルと内容の辞書型にする
        /// </summary>
        /// <param name="data">解凍済みのIceのデータストリーム</param>
        /// <param name="isGroupOne">グループ１のファイルか</param>
        /// <returns>グループ別ファイルリスト</returns>
        public static List<IceEntryModel> IceToFileList(byte[][] data, bool isGroupOne = false)
        {
            List<IceEntryModel> fileList = new ();
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
                    Group = isGroupOne ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP1,
                });

                data[index] = null;
            }

            return fileList;
        }

        /// <summary>
        /// ファイルリストをIceEntryModelにする
        /// </summary>
        /// <param name="files">ファイル一覧</param>
        /// <returns>IceEntryModel</returns>
        public List<IceEntryModel> FileListToIce(string[] files)
        {
            List<IceEntryModel> ret = new ();

            // 入力ディレクトリ内のファイルを走査
            foreach (string fileName in files)
            {
                // ファイルをバイト配列として読み込む
                ret.Add(new IceEntryModel()
                {
                    FileName = Path.GetFileName(fileName),
                    Content = File.ReadAllBytes(fileName),
                    Group = whiteList.Contains(fileName) ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP2,
                });
            }

            return ret;
        }

        /// <summary>
        /// ファイルリストをIceEntryModelにする（非同期）
        /// </summary>
        /// <param name="files">ファイル一覧</param>
        /// <returns>IceEntryModel</returns>
        public async Task<List<IceEntryModel>> FileListToIceAsync(string[] files)
        {
            List<IceEntryModel> ret = new ();

            // 入力ディレクトリ内のファイルを走査
            foreach (string fileName in files)
            {
                // ファイルをバイト配列として読み込む
                ret.Add(new IceEntryModel()
                {
                    FileName = Path.GetFileName(fileName),
                    Content = await File.ReadAllBytesAsync(fileName),
                    Group = whiteList.Contains(fileName) ? IceGroupEnum.GROUP1 : IceGroupEnum.GROUP2,
                });
            }

            return ret;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZamboniService"/> class.
        /// </summary>
        /// <param name="localizeService">多言語化サービス</param>
        /// <param name="eventAggregator">イベントサービス</param>
        public ZamboniService(ILocalizeService localizeService, IEventAggregator eventAggregator)
        {
            // 設定からホワイトリストを読み込む
            this.whiteList = new List<string>(Properties.Settings.Default.WhiteList.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));

            this.localizeService = localizeService;
            this.eventAggregator = eventAggregator;

            progressDialog = new Views.ProgressDialog();
        }

        /// <summary>
        /// 指定されたディレクトリをパックする
        /// </summary>
        /// <param name="inputPath">パックしたいファイルのディレクトリ</param>
        /// <param name="recursive">サブディレクトリを再帰的に含めるか</param>
        /// <param name="compress">圧縮する（非推奨）</param>
        /// <param name="forceUnencrypted">暗号化する（非推奨）</param>
        /// <returns>パックしたファイルのバイナリ</returns>
        public async Task<byte[]> Pack(string inputPath, bool recursive = true, bool compress = false, bool forceUnencrypted = false)
        {
            // ディレクトリの存在チェック
            if (!Directory.Exists(inputPath))
            {
                throw new DirectoryNotFoundException("ZamboniService: Input directory is not found.");
            }

            _ = progressDialog.ShowAsync();

            // ファイル一覧を取得
            string[] files = recursive ? Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories).ToArray() : Directory.GetFiles(inputPath);

            // グループ1に書き込むバイナリデータ
            List<byte[]> group1Binaries = new ();

            // グループ2に書き込むバイナリデータ
            List<byte[]> group2Binaries = new ();

            // 入力ディレクトリ内のファイルを走査
            foreach (string currentFile in files)
            {
                // 入力ファイル
                string fileName = Path.GetFileName(currentFile);

                // int index = files.IndexOf(currentFile);

                // ファイルをバイト配列として読み込む
                List<byte> file = new (await File.ReadAllBytesAsync(currentFile));

                // ヘッダを書き込む
                file.InsertRange(0, new IceFileHeader(currentFile, (uint)file.Count).GetBytes());

                // グループ1と2でファイルを振り分ける
                // 本プログラムでは設定画面から振り分けるファイルを定義する。
                //
                // ※PSO2のデータファイルは、拡張子やファイル名に応じてグループ1とグループ2で振り分けて書き込まれる。
                // 　たとえばテスクチャなどの.ddsファイルはグループ1に書き込まれるが、
                // 　モデルやポリゴンを定義するaqoファイルなどはグループ2に書き込まれないと動かない。
                // 　ちなみに、グループ1に書き込まれるoxyresource.crcは、Modを有効化させるとき必要だぞ。
                if (whiteList.Contains(fileName))
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
            IceArchiveHeader header = new ();

            // Iceファイルとして書き出す
            IceV4File ice = new (header.GetBytes(), group1Binaries.ToArray(), group2Binaries.ToArray());

            byte[] ret = ice.getRawData(compress, forceUnencrypted);

            progressDialog.Hide();

            return ret;
        }

        /// <inheritdoc/>
        public async void Unpack(string inputPath, string outputPath = null, bool createSubDirectory = true, bool separate = false)
        {
            _ = progressDialog.ShowAsync();

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }

            // Iceファイルを読み込む
            IceFile iceFile = LoadIceFile(inputPath);

            // 出力先のディレクトリ（ファイル名_ext）　※repack_ice.exeと同じ仕様
            string destination = outputPath + (createSubDirectory ? Path.DirectorySeparatorChar + Path.GetFileName(inputPath) + "_ext" : string.Empty) + Path.DirectorySeparatorChar;

            // グループ1
            List<IceEntryModel> groupOneFiles = IceToFileList(iceFile.groupOneFiles, true);

            // グループ2
            List<IceEntryModel> groupTwoFiles = IceToFileList(iceFile.groupTwoFiles);

            // 結合
            groupOneFiles.AddRange(groupTwoFiles);

            // 出力できるファイルがない場合
            if (groupOneFiles.Count == 0)
            {
                throw new Exception($"Neither group1 nor group2 was dumped from {Path.GetFileName(inputPath)}.");
            }

            if (createSubDirectory)
            {
                // 出力先のディレクトリ作成
                if (!Directory.Exists(destination))
                {
                    _ = Directory.CreateDirectory(destination);
                }

                if (separate)
                {
                    if (!Directory.Exists(destination + Path.DirectorySeparatorChar + "group1"))
                    {
                        _ = Directory.CreateDirectory(destination + Path.DirectorySeparatorChar + "group1");
                    }

                    if (!Directory.Exists(destination + Path.DirectorySeparatorChar + "group2"))
                    {
                        _ = Directory.CreateDirectory(destination + Path.DirectorySeparatorChar + "group2");
                    }
                }
            }

            foreach (IceEntryModel model in groupOneFiles)
            {
                // int index = groupOneFiles.IndexOf(model);

                // ファイル名（ひどい可読性だ）
                string fileName = destination + (separate ?
                                                  (model.Group == IceGroupEnum.GROUP1 ? "group1" : "group2") + Path.DirectorySeparatorChar : string.Empty)
                                              + model.FileName;

                try
                {
                    await File.WriteAllBytesAsync(fileName, model.Content);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                /*
                using BinaryWriter writer = new(new FileStream(fileName, FileMode.Create));
                await writer.WriteAsync(model.Content);
                writer.Close();
                */
            }

            progressDialog.Hide();
        }

        /// <inheritdoc/>
        public async Task<List<string>> FileList(string inputPath)
        {
            _ = progressDialog.ShowAsync();
            List<string> ret = new ();
            List<string> entries = new (Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories));
            Debug.WriteLine("Entries: ", entries.Count);

            // CSVのヘッダ
            ret.Add("filename,version,group,content");
            foreach (string path in entries)
            {
                Debug.WriteLine(path);

                if (path.Contains('.'))
                {
                    continue;
                }

                // Iceファイルをバイトとして読み込む
                byte[] buffer = await File.ReadAllBytesAsync(path);

                // Iceファイルのヘッダチェック
                if (IsNotIce(buffer))
                {
                    continue;
                }

                // メモリーストリームを生成
                using MemoryStream ms = new (buffer);

                // ヘッダを確認
                _ = ms.Seek(8L, SeekOrigin.Begin);
                int num = ms.ReadByte();
                _ = ms.Seek(0L, SeekOrigin.Begin);

                FileInfo fileInfo = new (path);

                // NGSのデータファイルの場合、親ディレクトリのパスも含める
                string ice =
                    (fileInfo.Directory.Name != "win32" ? fileInfo.Directory.Name + Path.DirectorySeparatorChar : string.Empty)
                        + fileInfo.Name + ",ICE" + num + ",";

                // Iceファイルを読み込む
                IceFile iceFile;
                try
                {
                    iceFile = IceFile.LoadIceFile(ms);
                }
                catch
                {
                    ret.Add(ice + "0,[ERROR] Could not load ice file.");
                    continue;
                }

                if (iceFile == null)
                {
                    ret.Add(ice + "0,[ERROR] Could not parse ice file.");
                    continue;
                }

                // グループ1のファイルをパース
                if (iceFile.groupOneFiles != null)
                {
                    try
                    {
                        iceFile.groupOneFiles.ForEach(bytes =>
                        {
                            int int32 = BitConverter.ToInt32(bytes, 16);
                            string str2 = Encoding.ASCII.GetString(bytes, 64, int32).TrimEnd(new char[1]);
                            ret.Add(ice + "1," + str2);
                        });
                    }
                    catch
                    {
                        ret.Add(ice + "1,[ERROR] Could not parse Group1 files.");
                    }
                }

                // グループ2のファイルをパース
                if (iceFile.groupTwoFiles != null)
                {
                    try
                    {
                        iceFile.groupTwoFiles.ForEach(bytes =>
                        {
                            int int32 = BitConverter.ToInt32(bytes, 16);
                            string str2 = Encoding.ASCII.GetString(bytes, 64, int32).TrimEnd(new char[1]);
                            ret.Add(ice + "2," + str2);
                        });
                    }
                    catch
                    {
                        ret.Add(ice + "2,[ERROR] Could not parse Group2 files.");
                    }
                }
            }

            progressDialog.Hide();
            return ret;
        }

        /// <inheritdoc/>
        public IceFile LoadIceFile(string inputPath)
        {
            // Iceファイルをバイトとして読み込む
            byte[] buffer = File.ReadAllBytes(inputPath);

            // IceFile.LoadIceFile(fs).header;

            // Iceファイルのヘッダチェック
            if (IsNotIce(buffer))
            {
                throw new Exception("Not ice file.");
            }

            // メモリーストリームを生成
            using MemoryStream ms = new (buffer);

            // Iceファイルを読み込む
            IceFile iceFile = IceFile.LoadIceFile(ms);
            return iceFile ?? throw new Exception("Could not parse ice file.");
        }

        /// <inheritdoc/>
        public async Task<IceFile> LoadIceFileAsync(string inputPath)
        {
            // Iceファイルをバイトとして読み込む
            byte[] buffer = await File.ReadAllBytesAsync(inputPath);

            // Iceファイルのヘッダチェック
            if (IsNotIce(buffer))
            {
                throw new Exception("Not ice file.");
            }

            // メモリーストリームを生成
            using MemoryStream ms = new (buffer);

            // Iceファイルを読み込む
            IceFile iceFile = IceFile.LoadIceFile(ms);
            return iceFile ?? throw new Exception("Could not parse ice file.");
        }
    }
}