// -----------------------------------------------------------------------
// <copyright file="CacheDbService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImTools;
using NgsPacker.Entities;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;

namespace NgsPacker.Services;

/// <summary>
///     ファイル名キャッシュサービス
/// </summary>
public class CacheDbService : ICacheDbService, IDisposable
{
    /// <summary>
    /// Zamboniサービス
    /// </summary>
    private readonly IZamboniService zamboniService;

    /// <summary>
    /// データファイルのある場所へのパス
    /// </summary>
    private readonly string dataPath = Properties.Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar;

    /// <summary>
    /// データベースのコンテキスト
    /// </summary>
    private CacheDbContext context = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheDbService" /> class.
    /// </summary>
    public CacheDbService(IZamboniService zamboniService)
    {
        context.Database.EnsureCreated();
        this.zamboniService = zamboniService;
    }

    /// <summary>
    ///     ファイナライズ
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async Task ScanFileｓ(bool force = false)
    {
        List<string> entries = new (Directory.EnumerateFiles(dataPath, "*.*", SearchOption.AllDirectories));
        Debug.WriteLine("Entries: ", entries.Count);

        // MD5計算機
        using MD5 md5 = MD5.Create();

        // CSVのヘッダ
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
            if (buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0)
            {
                // Iceファイルでない場合はスキップ
                continue;

            }

            // MD5ハッシュ
            byte[] hash = md5.ComputeHash(buffer);

            StringBuilder sb = new ();

            foreach (byte b in hash)
            {
                // バイト配列のハッシュを文字列に変換
                sb.Append(b.ToString("X2"));
            }

            // ファイルのハッシュ
            string fileHash = sb.ToString();

            // ファイルのエントリ
            var fileEntry = context.Files.Single(x => x.Name == path);

            if (fileEntry.List().IsEmpty)
            {
                // 新規登録
                await context.Files.AddAsync(new Files()
                {
                    Name = path,
                    Hash = fileHash,
                    UpdatedAt = File.GetLastWriteTime(path),
                });
            }
            else
            {
                // ファイルは保存されているが、ハッシュが異なる場合
                if (fileEntry.Hash != fileHash)
                {
                    // ハッシュと更新日時を更新
                    fileEntry.Hash = fileHash;
                    fileEntry.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
            }
        }
    }

    /// <inheritdoc />
    public Task ScanContents(DataDirectoryType target = DataDirectoryType.All)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// コンテキストを破棄する
    /// </summary>
    /// <param name="disposing">破棄中か</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing && context == null)
        {
            return;
        }

        context.Dispose();
        context = null;
    }

}