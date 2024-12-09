// -----------------------------------------------------------------------
// <copyright file="ZamboniService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using DryIoc.ImTools;
using FastSearchLibrary;
using NgsPacker.Events;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using NgsPacker.Models;
using NgsPacker.Properties;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zamboni;
using Zamboni.IceFileFormats;
using static Zamboni.IceFileFormats.IceHeaderStructures;
using Path = System.IO.Path;

namespace NgsPacker.Services;

/// <summary>
///     ZamboniLibサービス
/// </summary>
public class ZamboniService : IZamboniService, IDisposable
{
    /// <summary>
    ///     中断トークンソース
    /// </summary>
    private readonly CancellationTokenSource cancellationTokenSource;

    /// <summary>
    ///     多言語化サービス
    /// </summary>
    private readonly ILocalizeService localizeService;

    /// <summary>
    ///     並列処理のオプション
    /// </summary>
    private readonly ParallelOptions parallelOptions;

    /// <summary>
    ///     進捗中断要求イベント
    /// </summary>
    private readonly ProgressCancelEvent progressCancelEvent;

    /// <summary>
    ///     進捗イベント
    /// </summary>
    private readonly ProgressEvent progressEvent;

    /// <summary>
    ///     中断トークン
    /// </summary>
    private CancellationToken cancellationToken;

    /// <summary>
    ///     破棄フラグ
    /// </summary>
    private bool disposedValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ZamboniService" /> class.
    /// </summary>
    /// <param name="localizeService">多言語化サービス</param>
    /// <param name="eventAggregator">イベントアグリエイター</param>
    public ZamboniService(ILocalizeService localizeService, IEventAggregator eventAggregator)
    {
        // 多言語サービス
        this.localizeService = localizeService;

        // 進捗イベント
        progressEvent = eventAggregator.GetEvent<ProgressEvent>();

        // 中断トークンを発行
        cancellationTokenSource = new CancellationTokenSource();

        // 並列処理設定
        parallelOptions = new ParallelOptions
        {
            // 最大スレッド数
            MaxDegreeOfParallelism = Settings.Default.MaxThreads,

            // 中断トークン
            CancellationToken = cancellationToken
        };

        // 進捗ダイアログのキャンセルボタンが押されたときにキャンセルフラグを立てる
        progressCancelEvent = eventAggregator.GetEvent<ProgressCancelEvent>();
        progressCancelEvent.Subscribe(() =>
        {
            cancellationTokenSource.Cancel();
        });
    }

    /// <summary>
    ///     グループ１のホワイトリスト
    /// </summary>
    private static List<string> WhiteList =>
        new(Settings.Default.WhiteList.Replace("\r\n", "\n").Split('\n', '\r'));

    // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    // ~ZamboniService()
    // {
    //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
    //     Dispose(disposing: false);
    // }

    /// <summary>
    ///     破棄
    /// </summary>
    public void Dispose()
    {
        // クラスを破棄するタイミングでキャンセル実行
        cancellationTokenSource.Cancel();

        // 破棄
        cancellationTokenSource.Dispose();

        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async Task<byte[]> Pack(string inputPath, bool recursive = true, bool compress = false,
        bool forceUnencrypted = false)
    {
        // ディレクトリの存在チェック
        if (!Directory.Exists(inputPath))
        {
            throw new DirectoryNotFoundException("ZamboniService: Input directory is not found.");
        }

        // 進捗モーダルを表示
        progressEvent.Publish(new ProgressEventModel
        {
            Caption = localizeService.GetLocalizedString("ProgressInitializeCaption"),
            Message = localizeService.GetLocalizedString("ProgressInitializeMessage"),
            Detail = inputPath,
            Animation = ProgressDialog.IPD_Animation.FileMultiple,
            IsIntermediate = true,
            IsVisible = true
        });

        // ファイル一覧を読み込む
        Task<List<FileInfo>> fileList = FileSearcher.GetFilesFastAsync(inputPath, "*.*");

        // グループ1に書き込むバイナリデータ
        List<byte[]> group1Binaries = new();

        // グループ2に書き込むバイナリデータ
        List<byte[]> group2Binaries = new();

        // 入力ディレクトリ内のファイルを走査
        List<FileInfo> files = await fileList;

        // 中断トークン発行
        cancellationToken = cancellationTokenSource.Token;

        progressEvent.Publish(new ProgressEventModel { IsVisible = false });

        files.ForEach(file =>
        {
            // 進捗モーダルの表示を更新
#pragma warning disable CA1305 // IFormatProvider を指定します
            progressEvent.Publish(new ProgressEventModel
            {
                Caption = localizeService.GetLocalizedString("ProgressPackCaption"),
                Message =
                    string.Format(
                        localizeService.GetLocalizedString("ProgressPackMessage"),
                        files.IndexOf(file), files.Count, inputPath),
                Detail = file.Name,
                Maximum = (uint)files.Count,
                Value = (uint)files.IndexOf(file),
                Animation = ProgressDialog.IPD_Animation.FileMove,
                IsIntermediate = false,
                IsVisible = true
            });
#pragma warning restore CA1305 // IFormatProvider を指定します

            // ファイルをバイト配列として読み込む
            List<byte> bytes = new(File.ReadAllBytes(file.FullName));

            // ヘッダを書き込む
            bytes.InsertRange(0, new IceFileHeader(file.FullName, (uint)bytes.Count).GetBytes());

            // グループ1と2でファイルを振り分ける
            // 本プログラムでは設定画面から振り分けるファイルを定義する。
            //
            // ※PSO2のデータファイルは、拡張子やファイル名に応じてグループ1とグループ2で振り分けて書き込まれる。
            // 　たとえばテスクチャなどの.ddsファイルはグループ1に書き込まれるが、
            // 　モデルやポリゴンを定義するaqoファイルなどはグループ2に書き込まれないと動かない。
            // 　ちなみに、グループ1に書き込まれるoxyresource.crcは、Modを有効化させるとき必要だぞ。
            if (WhiteList.Contains(file.Name))
            {
                // グループ1に分類されるファイル
                group1Binaries.Add(bytes.ToArray());
            }
            else
            {
                // グループ2に分類されるファイル
                group2Binaries.Add(bytes.ToArray());
            }
        });

        // 進捗モーダルの表示を更新
        progressEvent.Publish(new ProgressEventModel { IsIntermediate = true });

        // ヘッダ
        IceArchiveHeader header = new();

        // Iceファイルとして書き出す
        IceV4File ice = new(header.GetBytes(), group1Binaries.ToArray(), group2Binaries.ToArray());

        progressEvent.Publish(new ProgressEventModel { IsVisible = false });

        return ice.getRawData(compress, forceUnencrypted);
    }

    /// <inheritdoc />
    public bool Unpack(string inputPath, string outputPath = null, bool createSubDirectory = true,
        bool separate = false)
    {
        progressEvent.Publish(new ProgressEventModel
        {
            Caption = localizeService.GetLocalizedString("ProgressInitializeCaption"),
            Message = localizeService.GetLocalizedString("ProgressInitializeMessage"),
            Detail = inputPath,
            Animation = ProgressDialog.IPD_Animation.FileMultiple,
            IsIntermediate = true,
            IsVisible = true
        });

        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        // Iceファイルを読み込む
        IceFile iceFile = IceUtility.LoadIceFile(inputPath);

        // 出力先のディレクトリ（ファイル名_ext）　※repack_ice.exeと同じ仕様
        string destination = outputPath +
                             (createSubDirectory
                                 ? Path.DirectorySeparatorChar + Path.GetFileName(inputPath) + "_ext"
                                 : string.Empty) + Path.DirectorySeparatorChar;

        // グループ1
        List<IceEntryModel> groupOneFiles = IceToFileList(iceFile.groupOneFiles, true);

        // グループ2
        List<IceEntryModel> groupTwoFiles = IceToFileList(iceFile.groupTwoFiles);

        // 結合
        groupOneFiles.AddRange(groupTwoFiles);

        // 出力できるファイルがない場合
        if (groupOneFiles.Count == 0)
        {
            throw new ArgumentException($"Neither group1 nor group2 was dumped from {Path.GetFileName(inputPath)}.");
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

        Debug.WriteLine("アンパック処理実行");

        try
        {
            groupOneFiles.ForEach(model =>
            {
                // ファイル名（ひどい可読性だ）
                string fileName = destination + (separate
                                                  ? (model.Group == IceGroup.Group1 ? "group1" : "group2") +
                                                    Path.DirectorySeparatorChar
                                                  : string.Empty)
                                              + model.FileName;

                // 進捗ダイアログを更新
#pragma warning disable CA1305 // IFormatProvider を指定します
                progressEvent.Publish(new ProgressEventModel
                {
                    Caption = localizeService.GetLocalizedString("ProgressUnpackCaption"),
                    Message =
                        string.Format(
                            localizeService.GetLocalizedString("ProgressUnpackMessage"),
                            groupOneFiles.IndexOf(model), groupOneFiles.Count, inputPath),
                    Detail = fileName,
                    Maximum = (uint)groupOneFiles.Count,
                    Value = (uint)groupOneFiles.IndexOf(model),
                    Animation = ProgressDialog.IPD_Animation.FileCopy,
                    IsIntermediate = false,
                    IsVisible = true
                });
#pragma warning restore CA1305 // IFormatProvider を指定します

                // キャンセルされてたら OperationCanceledException を投げるメソッド
                cancellationToken.ThrowIfCancellationRequested();

                File.WriteAllBytesAsync(fileName, model.Content, cancellationToken);
            });
        }
        catch (IOException)
        {
            throw new IOException("ZamboniService: Could not write Unpacked file. ");
        }
        catch (OperationCanceledException)
        {
            // Trough
        }
        finally
        {
            progressEvent.Publish(new ProgressEventModel { IsVisible = false });
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<List<string>> FileList(DataDirectoryType target)
    {
        // ディレクトリ内ファイル一覧
        Task<List<FileInfo>> fileList = IceUtility.GetTargetFiles(target);

        // 入力ディレクトリ内のファイルを走査
        List<FileInfo> files = await fileList;

        Debug.WriteLine("Entries: ", files.Count);

        // CSVデータ
        List<string> ret = new()
        {
            // CSVのヘッダ
            "filename,version,group,content"
        };

        // 中断トークン発行
        cancellationToken = cancellationTokenSource.Token;

        try
        {
            ParallelLoopResult result = Parallel.ForEach(files, parallelOptions, (file, loopState) =>
            {
                // エントリ名
                string entryName = IceUtility.GetEntryName(file.FullName);

                // 進捗ダイアログを更新
#pragma warning disable CA1305 // IFormatProvider を指定します
                progressEvent.Publish(new ProgressEventModel
                {
                    Caption = localizeService.GetLocalizedString("ProgressFileListCaption"),
                    Message =
                        string.Format(
                            localizeService.GetLocalizedString("ProgressFileListMessage"),
                            files.IndexOf(file), files.Count),
                    Detail = entryName,
                    Maximum = (uint)files.Count,
                    Value = (uint)files.IndexOf(file),
                    Animation = ProgressDialog.IPD_Animation.FileMultiple,
                    IsIntermediate = false,
                    IsVisible = true
                });
#pragma warning restore CA1305 // IFormatProvider を指定します

                // キャンセルされてたら OperationCanceledException を投げるメソッド
                cancellationToken.ThrowIfCancellationRequested();
                string result = await Analyze(file.FullName);

                // 解析実行
                ret.Add(result);
            });
            if (result.IsCompleted)
            {
                Debug.WriteLine("ループ処理がすべて終了した");
            }
        }
        catch (OperationCanceledException)
        {
            return ret;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            // return new List<string>();
        }
        finally
        {
            progressEvent.Publish(new ProgressEventModel { IsVisible = false });
            cancellationToken = CancellationToken.None;
        }

        return ret;
    }

    /// <summary>
    ///     破棄
    /// </summary>
    /// <param name="disposing">破棄中か</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                progressEvent.Unsubscribe(null);
                progressCancelEvent.Unsubscribe(null);
                cancellationTokenSource.Dispose();
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            // TODO: 大きなフィールドを null に設定します
            disposedValue = true;
        }
    }

    /// <summary>
    ///     ICEファイル内のファイルリストを出力
    /// </summary>
    /// <param name="file">解析対象ファイル</param>
    /// <returns>CSVの行</returns>
    private static async Task<string> Analyze(string file)
    {
        // TODO: ここの処理は後日SQLを保存する実装にする

        // ファイルストリームとして読み込み
        // Debug.WriteLine(file);
        await using FileStream fs = new(file, FileMode.Open, FileAccess.Read);

        // メモリーストリームを準備
        using MemoryStream ms = new();

        string name = IceUtility.GetEntryName(file);

        byte[] data = ms.ToArray();

        // ICEファイルのチェック
        if (!IceUtility.IsIceFile(data))
        {
            // ICEファイルでない場合、マジックナンバーを記入
            // https://techblog.sega.jp/entry/2016/12/26/100000
            CsvModel line = new();
            line.FileName = name;
            line.Group = 0;
            try
            {
                line.Magic = Encoding.UTF8.GetString(data, 0, 4);
                line.Content = "Not an ICE file. Maybe raw game data.";
            }
            catch (ArgumentOutOfRangeException)
            {
                line.Magic = "???";
                line.Content = "Could not detect magic number.";
                Debug.WriteLine("Could not detect magic number: " + name);
            }

            return line.ToString();
        }

        // ヘッダを確認
        _ = ms.Seek(8L, SeekOrigin.Begin);
        int num = ms.ReadByte();
        _ = ms.Seek(0L, SeekOrigin.Begin);

        // Iceファイルを読み込む
        IceFile iceFile;
        try
        {
            iceFile = IceFile.LoadIceFile(ms);
        }
        catch (Exception e)
        {
            // Iceファイルが読み込めない
            CsvModel line = new();
            line.FileName = name;
            line.Magic = "ICE" + num;
            line.Group = 0;
            line.Content = "[ZamboniError] ";
            Debug.WriteLine("ZamboniError: " + e.Message);
            return line.ToString();
        }
        finally
        {
            // メモリーストリームを破棄
            await ms.DisposeAsync();
        }

        // CSVファイルの行テキストを作成
        StringBuilder sb = new();

        // グループ1のファイルをパース
        iceFile.groupOneFiles?.ForEach(bytes =>
        {
            CsvModel line = new();
            line.FileName = name;
            line.Magic = "ICE" + num;
            line.Group = 1;
            line.Content = Encoding.ASCII.GetString(bytes, 64, BitConverter.ToInt32(bytes, 16)).TrimEnd(new char[1]);
            sb.AppendLine(line.ToString());
        });

        // グループ2のファイルをパース
        iceFile.groupTwoFiles?.ForEach(bytes =>
        {
            CsvModel line = new();
            line.FileName = name;
            line.Magic = "ICE" + num;
            line.Group = 2;
            line.Content = Encoding.ASCII.GetString(bytes, 64, BitConverter.ToInt32(bytes, 16)).TrimEnd(new char[1]);
            sb.AppendLine(line.ToString());
        });

        return sb.ToString(); // .TrimEnd(Environment.NewLine.ToCharArray());
    }

    /// <summary>
    ///     グループをファイルと内容の辞書型にする
    /// </summary>
    /// <param name="data">解凍済みのIceのデータストリーム</param>
    /// <param name="isGroupOne">グループ１のファイルか</param>
    /// <returns>グループ別ファイルリスト</returns>
    private static List<IceEntryModel> IceToFileList(byte[][] data, bool isGroupOne = false)
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
            fileList.Add(new IceEntryModel
            {
                FileName = Encoding.ASCII.GetString(data[index], 64, int32).TrimEnd(new char[1]),
                Content = content,
                Group = isGroupOne ? IceGroup.Group1 : IceGroup.Group2
            });

            data[index] = null;
        }

        return fileList;
    }

    /// <summary>
    ///     ファイルリストをIceEntryModelにする
    /// </summary>
    /// <param name="files">ファイル一覧</param>
    /// <param name="token">中断トークン</param>
    /// <returns>IceEntryModelのリスト</returns>
    private static async Task<List<IceEntryModel>> FileListToIce(string[] files, CancellationToken token)
    {
        List<IceEntryModel> ret = new();

        // 入力ディレクトリ内のファイルを走査
        foreach (string fileName in files)
        {
            // ファイルをバイト配列として読み込む
            ret.Add(new IceEntryModel
            {
                FileName = Path.GetFileName(fileName),
                Content = await File.ReadAllBytesAsync(fileName, token),
                Group = WhiteList.Contains(fileName) ? IceGroup.Group1 : IceGroup.Group2
            });
        }

        return ret;
    }
}
