// -----------------------------------------------------------------------
// <copyright file="IProgressContentDialogService.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NgsPacker.Services
{
    public interface IProgressContentDialogService
    {
        Task ShowAsync();
        void Hide();
    }
}
