// -----------------------------------------------------------------------
// <copyright file="LocalizeService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace NgsPacker.Services;

/// <summary>
///     多言語化サービス
/// </summary>
public class LocalizeService : ILocalizeService
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LocalizeService" /> class.
    /// </summary>
    /// <param name="locale">The locale<see cref="string" />.</param>
    public LocalizeService(string locale = null)
    {
        locale ??= CultureInfo.CurrentCulture.ToString();

        Debug.WriteLine(CultureInfo.GetCultures(CultureTypes.AllCultures));

        SupportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(
                c =>
                    c.IetfLanguageTag.Equals("en-US", StringComparison.Ordinal) ||
                    c.IetfLanguageTag.Equals("ja-JP", StringComparison.Ordinal))
            .ToList();
        SetLocale(locale);
    }

    /// <inheritdoc />
    public IList<CultureInfo> SupportedLanguages { get; }

    /// <inheritdoc />
    public CultureInfo SelectedLanguage { get => LocalizeDictionary.Instance.Culture; set => SetLocale(value); }

    /// <inheritdoc />
    public void SetLocale(string locale)
    {
        LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(locale);
    }

    /// <inheritdoc />
    public void SetLocale(CultureInfo culture)
    {
        LocalizeDictionary.Instance.Culture = culture;
    }

    /// <inheritdoc />
    public string GetLocalizedString(string key)
    {
        LocExtension locExtension = new(key);
        _ = locExtension.ResolveLocalizedValue(out string uiString);
        return uiString;
    }
}
