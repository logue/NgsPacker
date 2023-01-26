// -----------------------------------------------------------------------
// <copyright file="Files.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace NgsPacker.Entities
{
    /// <summary>
    /// ファイルのエンティティ
    /// </summary>
    public class Files
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// ファイルのハッシュ
        /// </summary>
        public string? Hash { get; set; }

        /// <summary>
        /// このレコードの更新日時
        /// </summary>
        public SqlDateTime UpdatedAt { get; set; }


        /// <summary>
        /// ファイルの内容物
        /// </summary>
        public List<Contents> Contents { get; } = new List<Contents>();
    }
}
