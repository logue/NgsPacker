// -----------------------------------------------------------------------
// <copyright file="ProgressModel.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace NgsPacker.Models
{
    /// <summary>
    /// 進捗情報
    /// </summary>
    public class ProgressModel
    {
        /// <summary>
        /// 進捗値
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 進捗テキスト
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressModel"/> class.
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="text">メッセージ</param>
        public ProgressModel(int value, string text)
        {
            Value = value;
            Message = text;
        }
    }
}
