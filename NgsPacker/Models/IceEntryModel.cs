// -----------------------------------------------------------------------
// <copyright file="IceEntryModel.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace NgsPacker.Models
{
    /// <summary>
    /// Iceをやり取りするときのモデル
    /// </summary>
    public class IceEntryModel
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// データ
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// グループ
        /// </summary>
        public IceGroupEnum Group { get; set; }
    }

    /// <summary>
    /// グループ
    /// </summary>
    public enum IceGroupEnum
    {
        /// <summary>
        /// グループ１のファイル
        /// </summary>
        GROUP1,

        /// <summary>
        /// グループ２のファイル
        /// </summary>
        GROUP2,
    }
}
