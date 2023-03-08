// -----------------------------------------------------------------------
// <copyright file="IProgressDialogService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace NgsPacker.Interfaces;

public interface IProgressDialogService
{
    void Show();
    void Close();
}
