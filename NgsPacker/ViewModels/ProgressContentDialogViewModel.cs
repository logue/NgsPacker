// -----------------------------------------------------------------------
// <copyright file="ProgressContentDialogViewModel.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Prism.Commands;
using Prism.Mvvm;

namespace NgsPacker.ViewModels
{
    public class ProgressContentDialogViewModel : BindableBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsInterminate { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Value { get; set; }
        public DelegateCommand CancelCommand { get; private set; }

        public ProgressContentDialogViewModel()
        {
            IsInterminate = true;
        }

        private void ExecuteProgressCommand()
        {

        }
    }
}
