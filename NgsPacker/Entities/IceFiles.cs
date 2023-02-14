// -----------------------------------------------------------------------
// <copyright file="IceFiles.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NgsPacker.Entities;

/// <summary>
///     ファイルのエンティティ
/// </summary>
public class IceFiles
{
    /// <summary>
    ///     ID
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    ///     ファイル名
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    ///     ファイルのMD5ハッシュ
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    ///     圧縮形式
    /// </summary>
    public int Format { get; set; }

    /// <summary>
    ///     ファイルの更新日時
    /// </summary>
    [Timestamp]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     ファイルの内容物
    /// </summary>
    [ForeignKey("IceFileId")]
    public List<Contents> Contents { get; } = new();
}
