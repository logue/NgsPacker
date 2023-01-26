// -----------------------------------------------------------------------
// <copyright file="AboutPageViewModel.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media;
using NgsPacker.Helper;
using NgsPacker.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace NgsPacker.ViewModels
{
    /// <summary>
    /// バージョン情報ページビューモデル
    /// </summary>
    public class AboutPageViewModel : BindableBase
    {
        /// <summary>
        /// プロジェクトサイト閲覧ボタンのコマンド.
        /// </summary>
        public DelegateCommand VisitCommand { get; }

        /// <summary>
        /// NexusModsサイト閲覧ボタンのコマンド.
        /// </summary>
        public DelegateCommand VisitNexusModsCommand { get; }

        /// <summary>
        /// ロゴ画像.
        /// </summary>
        public ImageSource Logo { get; set; }

        /// <summary>
        /// アセンブリ情報モデル.
        /// </summary>
        public AppAssemblyModel Assembly { get; }

        /// <summary>
        /// リンク
        /// </summary>
        /// <param name="url">URL</param>
        private static void Go(string url)
        {
            try
            {
                _ = Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windowsのとき
                    url = url.Replace("&", "^&");
                    _ = Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Linuxのとき
                    _ = Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Macのとき
                    _ = Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
        /// </summary>
        public AboutPageViewModel()
        {
            VisitCommand = new DelegateCommand(ExecuteVisitCommand);
            VisitNexusModsCommand = new DelegateCommand(ExecuteVisitNexusModsCommand);
            Assembly = new AppAssemblyModel();

            Logo = BitmapToImageSource.Convert(Properties.Resources.AppIcon);
        }

        /// <summary>
        /// プロジェクトサイト閲覧ボタンを実行.
        /// </summary>
        private void ExecuteVisitCommand()
        {
            Go("https://github.com/logue/NgsPacker");
        }

        /// <summary>
        /// NexusModsへ
        /// </summary>
        private void ExecuteVisitNexusModsCommand()
        {
            Go("https://www.nexusmods.com/phantasystaronline2newgenesis/mods/26");
        }
    }
}
