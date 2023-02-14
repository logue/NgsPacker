// -----------------------------------------------------------------------
// <copyright file="IceEntryModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace NgsPacker.Models;

/// <summary>
///     グループ列挙体
/// </summary>
public enum IceGroup
{
    /// <summary>
    ///     グループ１のファイル
    /// </summary>
    Group1,

    /// <summary>
    ///     グループ２のファイル
    /// </summary>
    Group2
}

/// <summary>
///     Iceファイルをやり取りするときの構造体モデル
/// </summary>
public class IceEntryModel
{
    /// <summary>
    ///     ファイル名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    ///     データ
    /// </summary>
    public byte[] Content { get; set; }

    /// <summary>
    ///     グループ
    /// </summary>
    public IceGroup Group { get; set; }
}
