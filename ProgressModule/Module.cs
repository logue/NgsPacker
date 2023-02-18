// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Models;
using NgsPacker.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace NgsPacker;

/// <summary>
///     進捗モーダルモジュール
/// </summary>
public class ProgressModule : IModule
{
    /// <summary>
    ///     モジュール初期化時
    /// </summary>
    /// <param name="containerProvider">/</param>
    public void OnInitialized(IContainerProvider containerProvider)
    {
        _ = containerProvider.Resolve<ProgressEventModel>();
    }

    /// <summary>
    ///     モジュール種別登録
    /// </summary>
    /// <param name="containerRegistry">.</param>
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        _ = containerRegistry.RegisterSingleton<ProgressEventModel>();
        containerRegistry.RegisterDialog<ProgressModal>();
    }
}
