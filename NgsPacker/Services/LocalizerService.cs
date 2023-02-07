// -----------------------------------------------------------------------
// <copyright file="LocalizerService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NgsPacker.Interfaces;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace NgsPacker.Services
{
    /// <summary>
    /// 多言語化サービス.
    /// </summary>
    public class LocalizerService : ILocalizerService
    {
        /// <summary>
        /// サポートされている言語.
        /// </summary>
        public IList<CultureInfo> SupportedLanguages { get; private set; }

        /// <summary>
        /// 現在選択されている言語.
        /// </summary>
        public CultureInfo SelectedLanguage { get => LocalizeDictionary.Instance.Culture; set => SetLocale(value); }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizerService"/> class.
        /// </summary>
        /// <param name="locale">The locale<see cref="string"/>.</param>
        public LocalizerService(string locale = null)
        {
            if (locale == null)
            {
                locale = CultureInfo.CurrentCulture.ToString();
            }

            System.Diagnostics.Debug.WriteLine(CultureInfo.GetCultures(CultureTypes.AllCultures));

            SupportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(
                c =>
                    c.IetfLanguageTag.Equals("en-US", System.StringComparison.Ordinal) ||
                    c.IetfLanguageTag.Equals("ja-JP", System.StringComparison.Ordinal)
            )
                .ToList();
            SetLocale(locale);
        }

        /// <summary>
        /// Set localization.
        /// </summary>
        /// <param name="locale">.</param>
        public void SetLocale(string locale)
        {
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(locale);
        }

        /// <summary>
        /// Set localization.
        /// </summary>
        /// <param name="culture">.</param>
        public void SetLocale(CultureInfo culture)
        {
            LocalizeDictionary.Instance.Culture = culture;
        }

        /// <summary>
        /// Get localized string from resource dictionary.
        /// </summary>
        /// <param name="key">.</param>
        /// <returns>.</returns>
        public string GetLocalizedString(string key)
        {
            LocExtension locExtension = new(key);
            _ = locExtension.ResolveLocalizedValue(out string uiString);
            return uiString;
        }
    }
}