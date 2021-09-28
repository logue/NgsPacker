// -----------------------------------------------------------------------
// <copyright file="AboutPageViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace NgsPacker.ViewModels
{
    public class AboutPageViewModel : BindableBase
    {
        /// <summary>
        /// Gets the VisitCommand
        /// プロジェクトサイト閲覧ボタンのコマンド.
        /// </summary>
        public DelegateCommand VisitCommand { get; }

        /// <summary>
        /// ロゴ画像.
        /// </summary>
        public ImageSource Logo { get; set; }

        /// <summary>
        /// アセンブリ情報モデル.
        /// </summary>
        public AppAssemblyModel Assembly { get; }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public AboutPageViewModel()
        {
            VisitCommand = new DelegateCommand(ExecuteVisitCommand);
            Assembly = new AppAssemblyModel();

            // Logo = BitmapToImageSource.Convert(Properties.Resources.AppIcon);
        }

        /// <summary>
        /// プロジェクトサイト閲覧ボタンを実行.
        /// </summary>
        private void ExecuteVisitCommand()
        {
            string url = "https://github.com/logue/NgsPacker";
            try
            {
                _ = Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //Windowsのとき
                    url = url.Replace("&", "^&");
                    _ = Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //Linuxのとき
                    _ = Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    //Macのとき
                    _ = Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
