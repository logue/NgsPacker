// -----------------------------------------------------------------------
// <copyright file="ProgressModalViewModel.cs" company="Logue">
// Copyright (c) 2021-2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ProgressModule.Models;

namespace ProgressModule.ViewModels
{
    /// <summary>
    /// 進捗モーダルのビューモデル
    /// </summary>
    public class ProgressModalViewModel : BindableBase, IDialogAware
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///  メッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 進捗
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// 進捗リングの表示制御
        /// </summary>
        public Visibility ProgressRingVisibility { get; set; }

        /// <summary>
        /// 進捗バーの表示制御
        /// </summary>
        public Visibility ProgressBarVisibility { get; set; }

        /// <summary>
        /// 閉じる
        /// </summary>
        public DelegateCommand CloseCommand { get; }

        /// <summary>
        /// 閉じるボタンが押された
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressModalViewModel"/> class.
        /// </summary>
        /// <param name="model">進捗モデル</param>
        public ProgressModalViewModel(ProgressModel model)
        {
            Progress = model.Progress;

            model.PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Progress":
                        Progress = model.Progress;
                        break;
                    case "Title":
                        Title = model.Title;
                        break;
                    case "Message":
                        Message = model.Message;
                        break;
                    case "isIntermediate":
                        // 中間状態のときはプログレスリングを表示
                        if (model.IsIntermediate)
                        {
                            ProgressRingVisibility = Visibility.Visible;
                            ProgressBarVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ProgressRingVisibility = Visibility.Collapsed;
                            ProgressBarVisibility = Visibility.Visible;
                        }
                        break;

                }
            };

            CloseCommand =
                new DelegateCommand
                (
                    () => CloseDialog(),
                    () => Progress >= 100
                )
                .ObservesProperty(() => Progress);
        }

        /// <summary>
        /// ダイアログを閉じることができるか
        /// </summary>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// ダイアログが閉じたとき
        /// </summary>
        public void OnDialogClosed()
        {
            Progress = 0;
        }

        /// <summary>
        /// ダイアログを開いたとき
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters) { }

        /// <summary>
        /// ダイアログを閉じるボタンが押されたとき
        /// </summary>
        private void CloseDialog()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
