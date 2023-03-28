// -----------------------------------------------------------------------
// <copyright file="IceUtility.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using FastSearchLibrary;
using NgsPacker.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Zamboni;

namespace NgsPacker.Helpers;

/// <summary>
///     Iceファイルに関するユーティリティ関数群
/// </summary>
public static class IceUtility
{
    /// <summary>
    ///     ICEファイルであるか
    /// </summary>
    /// <param name="buffer">バイト配列</param>
    /// <returns>ICEがマジックコードに含まれていたらtrue</returns>
    public static bool IsIceFile(byte[] buffer)
    {
        return !(buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0);
    }

    /// <summary>
    ///     データファイルのある場所へのパス
    /// </summary>
    /// <returns>dataディレクトリのパス</returns>
    public static string GetDataDir()
    {
        if (!File.Exists(Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "pso2.exe"))
        {
            throw new FileNotFoundException("pso2.exe does not found. Are the pso2 directory settings correct?");
        }

        return Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "data" +
               Path.DirectorySeparatorChar;
    }

    /// <summary>
    ///     対象のパスかの判定
    /// </summary>
    /// <param name="path">対象パス</param>
    /// <param name="target">対象ディレクトリ</param>
    /// <returns>対象だった場合true、そうでない場合false。licenseディレクトリは常にfalse</returns>
    public static bool IsTargetPath(DirectoryInfo path, DataDirectoryType target = DataDirectoryType.Ngs)
    {
        switch (target)
        {
            case DataDirectoryType.Pso:
                // PSO2ディレクトリのみの場合
                return Regex.IsMatch(path.Name, "win32") ||
                       Regex.IsMatch(path.Name, "win32_na");
            case DataDirectoryType.Ngs:
                // NGSディレクトリのみの場合
                return Regex.IsMatch(path.Name, "win32reboot") ||
                       Regex.IsMatch(path.Name, "win32reboot_na");
            default:
            case DataDirectoryType.All:
                // すべて対象にする場合
                break;
        }

        // ライセンスディレクトリは対象外
        return !Regex.IsMatch(path.Name, "license");
    }

    /// <summary>
    ///     入力パスからdataディレクトリまでのパスを抜いた値を返す（win32_reboot\00\112da290e9b607c4ab330e4a9103e1）
    /// </summary>
    /// <param name="inputPath">入力フルパス</param>
    /// <returns>エントリ名</returns>
    public static string GetEntryName(string inputPath)
    {
        return inputPath.Replace(GetDataDir(), string.Empty);
    }

    /// <summary>
    ///     対象ディレクトリのファイル一覧を取得
    /// </summary>
    /// <param name="target">対象ディレクトリ</param>
    /// <returns>ファイルリスト</returns>
    public static async Task<List<FileInfo>> GetTargetFiles(
        DataDirectoryType target = DataDirectoryType.Ngs)
    {
        // Dataディレクトリのパス
        string dataDir = GetDataDir();
        // 対象パス
        List<FileInfo> path = new();

        if (target == DataDirectoryType.All || target == DataDirectoryType.Pso)
        {
            // PSO2ディレクトリ
            path.AddRange(await FileSearcher.GetFilesFastAsync(dataDir + "win32", "*.*"));
            if (Directory.Exists(dataDir + "win32_na"))
            {
                path.AddRange(await FileSearcher.GetFilesFastAsync(dataDir + "win32_na", "*.*"));
            }
        }

        if (target == DataDirectoryType.All || target == DataDirectoryType.Ngs)
        {
            // NGSディレクトリ
            path.AddRange(await FileSearcher.GetFilesFastAsync(dataDir + "win32reboot", "*.*"));
            if (Directory.Exists(dataDir + "win32reboot_na"))
            {
                path.AddRange(await FileSearcher.GetFilesFastAsync(dataDir + "win32reboot_na", "*.*"));
            }
        }

        return path;
    }

    /// <summary>
    ///     Iceファイルを読み込む（非同期）
    /// </summary>
    /// <param name="inputPath">Iceファイルへのパス</param>
    /// <param name="token">中断トークン</param>
    /// <returns>Iceファイルのオブジェクト</returns>
    /// <exception cref="ArgumentException">パースできなかった場合</exception>
    public static async Task<IceFile> LoadIceFile(string inputPath, CancellationToken token)
    {
        // Iceファイルをバイトとして読み込む
        byte[] buffer = await File.ReadAllBytesAsync(inputPath, token);

        // Iceファイルのヘッダチェック
        if (!IsIceFile(buffer))
        {
            throw new ArgumentException("Not ice file.");
        }

        // メモリーストリームを生成
        using MemoryStream ms = new(buffer);

        // Iceファイルを読み込む
        IceFile iceFile = IceFile.LoadIceFile(ms);
        return iceFile ?? throw new ArgumentException("Could not parse ice file.");
    }

    /// <summary>
    ///     Iceファイルを読み込む（同期）
    /// </summary>
    /// <param name="inputPath">Iceファイルへのパス</param>
    /// <returns>Iceファイルのオブジェクト</returns>
    /// <exception cref="ArgumentException">パースできなかった場合</exception>
    public static IceFile LoadIceFile(string inputPath)
    {
        // Iceファイルをバイトとして読み込む
        byte[] buffer = File.ReadAllBytes(inputPath);

        // Iceファイルのヘッダチェック
        if (!IsIceFile(buffer))
        {
            throw new ArgumentException("Not ice file.");
        }

        // メモリーストリームを生成
        using MemoryStream ms = new(buffer);

        // Iceファイルを読み込む
        IceFile iceFile = IceFile.LoadIceFile(ms);
        return iceFile ?? throw new ArgumentException("Could not parse ice file.");
    }
}
