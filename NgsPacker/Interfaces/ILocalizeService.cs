// -----------------------------------------------------------------------
// <copyright file="ILocalizeService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;

namespace NgsPacker.Interfaces;

/// <summary>
///     多言語化サービスのインターフェース.
/// </summary>
public interface ILocalizeService
{
    /// <summary>
    ///     利用可能な言語
    /// </summary>
    IList<CultureInfo> SupportedLanguages { get; }

    /// <summary>
    ///     現在の言語
    /// </summary>
    CultureInfo SelectedLanguage { get; set; }

    /// <summary>
    ///     ロケールから言語を設定する
    /// </summary>
    /// <param name="locale">ロケール</param>
    void SetLocale(string locale);

    /// <summary>
    ///     カルチャーから言語を設定する
    /// </summary>
    /// <param name="culture">カルチャー</param>
    void SetLocale(CultureInfo culture);

    /// <summary>
    ///     キーから翻訳文を取得
    /// </summary>
    /// <param name="key">翻訳キー</param>
    /// <returns>翻訳文</returns>
    string GetLocalizedString(string key);
}
