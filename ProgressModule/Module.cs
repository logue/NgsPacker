// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Ioc;
using Prism.Modularity;
using ProgressModule.Models;
using ProgressModule.Views;

namespace ProgressModule
{
    /// <summary>
    /// 進捗モーダルモジュール
    /// </summary>
    public class Module : IModule
    {
        /// <summary>
        /// モジュール初期化時
        /// </summary>
        /// <param name="containerProvider"></param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _ = containerProvider.Resolve<ProgressModel>();
        }

        /// <summary>
        /// モジュール種別登録
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.RegisterSingleton<ProgressModel>();
            containerRegistry.RegisterDialog<ProgressModal>();
        }
    }
}