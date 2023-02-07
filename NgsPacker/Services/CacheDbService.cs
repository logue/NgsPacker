// -----------------------------------------------------------------------
// <copyright file="CacheDbService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;

namespace NgsPacker.Services
{

    /// <summary>
    /// ファイル名キャッシュサービス
    /// </summary>
    public class CacheDbService : ICacheDbService, IDisposable
    {
        /// <summary>
        /// データベースのコンテキスト
        /// </summary>
        private readonly CacheDbContext context = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDbService"/> class.
        /// </summary>
        public CacheDbService()
        {
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>

        public void Dispose()
        {
            // DBを開放
            context.Dispose();
        }
    }
}
