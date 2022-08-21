// -----------------------------------------------------------------------
// <copyright file="ProgressModel.cs" company="Logue">
// Copyright (c) 2022 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Events;
using Prism.Mvvm;

namespace ProgressModule.Models
{
    /// <summary>
    /// 進捗モデル
    /// </summary>
    public class ProgressModel : BindableBase
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
        /// 中間状態（リンクを表示）
        /// </summary>
        public bool IsIntermediate { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventAggregator"></param>
        public ProgressModel(IEventAggregator eventAggregator)
        {
            Progress = 0;

            eventAggregator
                .GetEvent<SetTitle>()
                .Subscribe(x => Title = x);
            eventAggregator
               .GetEvent<SetMessage>()
               .Subscribe(x => Message = x);
            eventAggregator
              .GetEvent<SetIntermediate>()
              .Subscribe(x => IsIntermediate = x);
            eventAggregator
                .GetEvent<SetProgress>()
                .Subscribe(x => Progress = x);
        }
    }
}
