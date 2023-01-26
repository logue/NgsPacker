// -----------------------------------------------------------------------
// <copyright file="NgsPackerDbContext.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using NgsPacker.Entities;

namespace NgsPacker
{
    /// <summary>
    /// DBの橋渡しを行うコンテキストクラス
    /// </summary>
    internal class NgsPackerDbContext : DbContext
    {
        /// <summary>
        /// ファイルレコード
        /// </summary>
        public DbSet<Files> Files { get; set; }

        /// <summary>
        /// ファイル内容物テーブル
        /// </summary>
        public DbSet<Contents> Contents { get; set; }

        /// <summary>
        /// DBのパス
        /// </summary>
        public string DbPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NgsPackerDbContext"/> class.
        /// </summary>
        public NgsPackerDbContext()
        {
            // DBファイルの保存先とDBファイル名
            DbPath = $"{Path.GetDirectoryName(Application.ExecutablePath)}NgsPacker.sqlite";
        }

        /// <summary>
        /// DBファイル生成
        /// </summary>
        /// <param name="options">オプション</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
