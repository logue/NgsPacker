// -----------------------------------------------------------------------
// <copyright file="ILocalizerService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;

namespace NgsPacker.Interfaces
{
    /// <summary>
    /// 多言語化サービスのインターフェース.
    /// </summary>
    public interface ILocalizerService
    {
        /// <summary>
        /// Set localization.
        /// </summary>
        /// <param name="locale">.</param>
        void SetLocale(string locale);

        /// <summary>
        /// Set localization.
        /// </summary>
        /// <param name="culture">.</param>
        void SetLocale(CultureInfo culture);

        /// <summary>
        /// Get a localized string by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>.</returns>
        string GetLocalizedString(string key);

        /// <summary>
        /// Supported languages.
        /// </summary>
        IList<CultureInfo> SupportedLanguages { get; }

        /// <summary>
        /// The current selected language.
        /// </summary>
        CultureInfo SelectedLanguage { get; set; }
    }
}