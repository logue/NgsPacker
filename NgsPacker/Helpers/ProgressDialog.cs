// -----------------------------------------------------------------------
// <copyright file="ProgressDialog.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ImTools;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NgsPacker.Helpers;

/// <summary>
///     進捗ダイアログ
/// </summary>
public class ProgressDialog : IDisposable
{
    /// <summary>
    ///     親ウィンドウのハンドル
    /// </summary>
    private readonly IntPtr parentHandle;

    /// <summary>
    ///     アニメーション
    /// </summary>
    private IPD_Animation animation = IPD_Animation.FileMove;

    /// <summary>
    ///     破棄フラグ
    /// </summary>
    private bool disposedValue;

    /// <summary>
    ///     キャプションの文字
    /// </summary>
    private string line1 = string.Empty;

    /// <summary>
    ///     メッセージ文
    /// </summary>
    private string line2 = string.Empty;

    /// <summary>
    ///     詳細
    /// </summary>
    private string line3 = string.Empty;

    /// <summary>
    ///     最大値
    /// </summary>
    private uint maximum = 100;

    /// <summary>
    ///     OS側の進捗ダイアログ
    /// </summary>
    private IWin32IProgressDialog pd;

    /// <summary>
    ///     ダイアログのタイトル
    /// </summary>
    private string title = string.Empty;

    /// <summary>
    ///     進捗
    /// </summary>
    private uint value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressDialog" /> class.
    /// </summary>
    public ProgressDialog()
    {
        parentHandle = Process.GetCurrentProcess().MainWindowHandle;
        if (parentHandle == IntPtr.Zero)
        {
            parentHandle = GetDesktopWindow();
        }

        Win32ProgressDialog win32ProgressDialog = new();
        pd = (IWin32IProgressDialog)win32ProgressDialog;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProgressDialog" /> class.
    /// </summary>
    /// <param name="parentHandle">親ウィンドウのハンドル</param>
    public ProgressDialog(IntPtr parentHandle)
    {
        this.parentHandle = parentHandle;
        // Reduced the lag of up to display the dialog displayed by force the function ShowWindow when uses Windows.Forms
        // This idea taken from http://rarara.cafe.coocan.jp/cgi-bin/lng/vc/vclng.cgi?print+200902/09020022.txt
        ShowWindow(this.parentHandle, SW_SHOWNORMAL);

        Win32ProgressDialog win32ProgressDialog = new();
        pd = (IWin32IProgressDialog)win32ProgressDialog;
    }

    /// <summary>
    ///     ウィンドウのタイトル
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            title = value;
            pd?.SetTitle(title);
        }
    }

    /// <summary>
    ///     ダイアログのキャプション（アニメーション動画のところに表示される）
    /// </summary>
    public string Caption
    {
        get => line1;
        set
        {
            line1 = value;
            pd?.SetLine(1, line1, false, IntPtr.Zero);
        }
    }

    /// <summary>
    ///     メッセージ
    /// </summary>
    public string Message
    {
        get => line2;
        set
        {
            line2 = value;
            pd?.SetLine(2, line2, false, IntPtr.Zero);
        }
    }

    /// <summary>
    ///     詳細（ファイル名など）
    /// </summary>
    public string Detail
    {
        get => line3;
        set
        {
            line3 = value;
            pd?.SetLine(3, line3, false, IntPtr.Zero);
        }
    }

    /// <summary>
    ///     進捗
    /// </summary>
    public uint Value
    {
        get => value;
        set
        {
            this.value = value;
            pd?.SetProgress(this.value, maximum);
        }
    }

    /// <summary>
    ///     最大値
    /// </summary>
    public uint Maximum
    {
        get => maximum;
        set
        {
            maximum = value;
            pd?.SetProgress(this.value, maximum);
        }
    }

    /// <summary>
    ///     ユーザがキャンセルボタンを押したか
    /// </summary>
    public bool HasUserCancelled
    {
        get
        {
            try
            {
                return pd.HasUserCancelled();
            }
            catch
            {
                return true;
            }
        }
    }

    /// <summary>
    ///     キャンセル時のメッセージ
    /// </summary>
    public string CancelMessage { get; set; }

    /// <summary>
    ///     アニメーション
    /// </summary>
    public IPD_Animation Animation
    {
        get => animation;
        set
        {
            animation = value;
            pd?.SetAnimation(parentHandle, animation);
        }
    }


    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                pd = null;
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            // TODO: 大きなフィールドを null に設定します
            disposedValue = true;
        }
    }

    // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    ~ProgressDialog()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(false);
    }

    /// <summary>
    ///     進捗ダイアログを表示
    /// </summary>
    /// <param name="flags" cref="IPD_Flags">フラグ</param>
    public void Open(params IPD_Flags[] flags)
    {
        pd.SetTitle(title);
        pd.SetLine(1, line1, false, IntPtr.Zero);
        pd.SetLine(2, line2, false, IntPtr.Zero);
        pd.SetLine(3, line3, false, IntPtr.Zero);
        pd.SetCancelMsg(CancelMessage, IntPtr.Zero);

        uint dialogFlags = (uint)IPD_Flags.Normal;
        if (flags.Length != 0)
        {
            flags.ForEach(flag => dialogFlags = dialogFlags | (uint)flag);
        }

        pd.SetAnimation(parentHandle, Animation);
        pd.StartProgressDialog(parentHandle, null, dialogFlags, IntPtr.Zero);
    }


    /// <summary>
    ///     進捗ダイアログを閉じる
    /// </summary>
    public void Close()
    {
        pd.StopProgressDialog();
    }

    #region "Win32 Stuff"

    [ComImport]
    [Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4")]
    internal class Win32ProgressDialog
    {
    }

    [ComImport]
    [Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWin32IProgressDialog
    {
        /// <summary>
        ///     Starts the progress dialog box.
        /// </summary>
        /// <param name="hwndParent">A handle to the dialog box's parent window.</param>
        /// <param name="punkEnableModless">Reserved. Set to null.</param>
        /// <param name="dwFlags">Flags that control the operation of the progress dialog box. </param>
        /// <param name="pvResevered">Reserved. Set to IntPtr.Zero</param>
        [PreserveSig]
        void StartProgressDialog
        (
            IntPtr hwndParent,
            [MarshalAs(UnmanagedType.IUnknown)] object punkEnableModless,
            uint dwFlags,
            IntPtr pvResevered
        );

        /// <summary>
        ///     Stops the progress dialog box and removes it from the screen.
        /// </summary>
        [PreserveSig]
        void StopProgressDialog();

        /// <summary>
        ///     Sets the title of the progress dialog box.
        /// </summary>
        /// <param name="pwzTitle">A pointer to a null-terminated Unicode string that contains the dialog box title.</param>
        [PreserveSig]
        void SetTitle
        (
            [MarshalAs(UnmanagedType.LPWStr)] string pwzTitle
        );

        /// <summary>
        ///     Specifies an Audio-Video Interleaved (AVI) clip that runs in the dialog box. Note: Note  This method is not
        ///     supported in Windows Vista or later versions.
        /// </summary>
        /// <param name="hInstAnimation">An instance handle to the module from which the AVI resource should be loaded.</param>
        /// <param name="idAnimation">
        ///     An AVI resource identifier. To create this value, use the MAKEINTRESOURCE macro. The control
        ///     loads the AVI resource from the module specified by hInstAnimation.
        /// </param>
        [PreserveSig]
        void SetAnimation
        (
            IntPtr hInstAnimation,
            IPD_Animation idAnimation
        );

        /// <summary>
        ///     Checks whether the user has canceled the operation.
        /// </summary>
        /// <returns>TRUE if the user has cancelled the operation; otherwise, FALSE.</returns>
        /// <remarks>
        ///     The system does not send a message to the application when the user clicks the Cancel button.
        ///     You must periodically use this function to poll the progress dialog box object to determine
        ///     whether the operation has been canceled.
        /// </remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool HasUserCancelled();

        /// <summary>
        ///     Updates the progress dialog box with the current state of the operation.
        /// </summary>
        /// <param name="dwCompleted">
        ///     An application-defined value that indicates what proportion of the operation has been
        ///     completed at the time the method was called.
        /// </param>
        /// <param name="dwTotal">
        ///     An application-defined value that specifies what value dwCompleted will have when the operation
        ///     is complete.
        /// </param>
        [PreserveSig]
        void SetProgress
        (
            uint dwCompleted,
            uint dwTotal
        );

        /// <summary>
        ///     Updates the progress dialog box with the current state of the operation.
        /// </summary>
        /// <param name="ullCompleted">
        ///     An application-defined value that indicates what proportion of the operation has been
        ///     completed at the time the method was called.
        /// </param>
        /// <param name="ullTotal">
        ///     An application-defined value that specifies what value ullCompleted will have when the operation
        ///     is complete.
        /// </param>
        [PreserveSig]
        void SetProgress64
        (
            ulong ullCompleted,
            ulong ullTotal
        );

        /// <summary>
        ///     Displays a message in the progress dialog.
        /// </summary>
        /// <param name="dwLineNum">
        ///     The line number on which the text is to be displayed. Currently there are three lines—1, 2, and
        ///     3. If the <see cref="IPD_Flags">IPD_Flags</see> flag was included in the dwFlags parameter when
        ///     IProgressDialog.StartProgressDialog was
        ///     called, only lines 1 and 2 can be used. The estimated time will be displayed on line 3.
        /// </param>
        /// <param name="pwzString">A null-terminated Unicode string that contains the text.</param>
        /// <param name="fCompactPath">
        ///     TRUE to have path strings compacted if they are too large to fit on a line. The paths are
        ///     compacted with PathCompactPath.
        /// </param>
        /// <param name="pvResevered"> Reserved. Set to IntPtr.Zero.</param>
        /// <remarks>
        ///     This function is typically used to display a message such as "Item XXX is now being processed." typically,
        ///     messages are displayed on lines 1 and 2, with line 3 reserved for the estimated time.
        /// </remarks>
        [PreserveSig]
        void SetLine
        (
            uint dwLineNum,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzString,
            [MarshalAs(UnmanagedType.VariantBool)] bool fCompactPath,
            IntPtr pvResevered
        );

        /// <summary>
        ///     Sets a message to be displayed if the user cancels the operation.
        /// </summary>
        /// <param name="pwzCancelMsg">A pointer to a null-terminated Unicode string that contains the message to be displayed.</param>
        /// <param name="pvResevered">Reserved. Set to NULL.</param>
        /// <remarks>
        ///     Even though the user clicks Cancel, the application cannot immediately call
        ///     IProgressDialog.StopProgressDialog to close the dialog box. The application must wait until the
        ///     next time it calls IProgressDialog.HasUserCancelled to discover that the user has canceled the
        ///     operation. Since this delay might be significant, the progress dialog box provides the user with
        ///     immediate feedback by clearing text lines 1 and 2 and displaying the cancel message on line 3.
        ///     The message is intended to let the user know that the delay is normal and that the progress dialog
        ///     box will be closed shortly.
        ///     It is typically is set to something like "Please wait while ...".
        /// </remarks>
        [PreserveSig]
        void SetCancelMsg
        (
            [MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg,
            IntPtr pvResevered
        );

        /// <summary>
        ///     Resets the progress dialog box timer to zero.
        /// </summary>
        /// <param name="dwTimerAction">Flags that indicate the action to be taken by the timer.</param>
        /// <param name="pvResevered">Reserved. Set to NULL.</param>
        /// <remarks>
        ///     The timer is used to estimate the remaining time. It is started when your application
        ///     calls IProgressDialog.StartProgressDialog. Unless your application will start immediately,
        ///     it should call Timer just before starting the operation.
        ///     This practice ensures that the time estimates will be as accurate as possible. This method
        ///     should not be called after the first call to IProgressDialog.SetProgress.
        /// </remarks>
        [PreserveSig]
        void Timer
        (
            IPDTIMER_Flags dwTimerAction,
            IntPtr pvResevered
        );
    }

    /// <summary>
    ///     Flags that indicate the action to be taken by the ProgressDialog.SetTime() method.
    /// </summary>
    public enum IPDTIMER_Flags : uint
    {
        /// <summary>Resets the timer to zero. Progress will be calculated from the time this method is called.</summary>
        Reset = 0x01,

        /// <summary>Progress has been suspended.</summary>
        Pause = 0x02,

        /// <summary>Progress has been resumed.</summary>
        Resume = 0x03
    }

    /// <summary>
    ///     進捗ダイアログのフラグ
    /// </summary>
    [Flags]
    public enum IPD_Flags : uint // DWORD
    {
        /// <summary>Normal progress dialog box behavior.</summary>
        Normal = 0x00000000,

        /// <summary>
        ///     The progress dialog box will be modal to the window specified by hwndParent. By default, a progress dialog box
        ///     is modeless.
        /// </summary>
        Modal = 0x00000001,

        /// <summary>Automatically estimate the remaining time and display the estimate on line 3. </summary>
        /// <remarks>If this flag is set, IProgressDialog::SetLine can be used only to display text on lines 1 and 2.</remarks>
        AutoTime = 0x00000002,

        /// <summary>Do not show the "time remaining" text.</summary>
        NoTime = 0x00000004,

        /// <summary>Do not display a minimize button on the dialog box's caption bar.</summary>
        NoMinimize = 0x00000008,

        /// <summary>Do not display a progress bar.</summary>
        /// <remarks>
        ///     Typically, an application can quantitatively determine how much of the operation remains and periodically pass
        ///     that value to IProgressDialog::SetProgress. The progress dialog box uses this information to update its progress
        ///     bar. This flag is typically set when the calling application must wait for an operation to finish, but does not
        ///     have any quantitative information it can use to update the dialog box.
        /// </remarks>
        NoProgressBar = 0x00000010,

        /// <summary>Use marquee progress.</summary>
        /// <remarks>comctl32 v6 required.</remarks>
        MarqueeProgress = 0x00000020,

        /// <summary>No cancel button (operation cannot be canceled).</summary>
        /// <remarks>Use sparingly.</remarks>
        NoCancel = 0x00000040,

        /// <summary>Add a pause button (operation can be paused)</summary>
        EnablePause = 0x00000080,

        /// <summary>The operation can be undone in the dialog.</summary>
        /// <remarks>The Stop button becomes Undo.</remarks>
        AllowUndo = 0x00000100,

        /// <summary>Don't display the path of source file in progress dialog.</summary>
        DontDisplaySourcePath = 0x00000200,

        /// <summary>Don't display the path of destination file in progress dialog.</summary>
        DontDisplayDistPath = 0x00000400
    }

    /// <summary>
    ///     File operation animations resource IDs in shell32.dll
    /// </summary>
    /// <see href="http://www.randomnoun.com/wp/2013/10/27/windows-shell32-animations/" />
    [Flags]
    public enum IPD_Animation : ushort
    {
        None = 0,

        /// <summary>
        ///     Torch over folder
        /// </summary>
        SearchFlashlight = 150,

        /// <summary>
        ///     Magnifying glass over document
        /// </summary>
        SearchDocument = 151,

        /// <summary>
        ///     Magnifying glass over monitor
        /// </summary>
        SearchComputer = 152,

        /// <summary>
        ///     Folder to folder file move
        /// </summary>
        FileMove = 160,

        /// <summary>
        ///     Folder to folder file copy
        /// </summary>
        FileCopy = 161,

        /// <summary>
        ///     Folder to recycle bin
        /// </summary>
        FileRecycleBin = 162,

        /// <summary>
        ///     Recycle bin file delete
        /// </summary>
        ClearRecycleBin = 163,

        /// <summary>
        ///     Folder file delete
        /// </summary>
        FileDelete = 164,

        /// <summary>
        ///     Set multiple file attributes
        /// </summary>
        FileMultiple = 165,

        /// <summary>
        ///     Magnifying glass over globe
        /// </summary>
        SearchGlobe = 166,

        /// <summary>
        ///     Folder to folder file move (Windows 9x style)
        /// </summary>
        FileMove9X = 167,

        /// <summary>
        ///     Folder to folder file copy (Windows 9x style)
        /// </summary>
        FileCopy9X = 168,

        /// <summary>
        ///     Folder file delete (Windows 9x style)
        /// </summary>
        FileDelete9X = 169,

        /// <summary>
        ///     Globe to folder download
        /// </summary>
        FileDownload = 170,
        NoAnimation = ushort.MaxValue
    }

    private const int SW_HIDE = 0;
    private const int SW_SHOWNORMAL = 1;
    private const int SW_NORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_MAXIMIZE = 3;
    private const int SW_SHOWNOACTIVATE = 4;
    private const int SW_SHOW = 5;
    private const int SW_MINIMIZE = 6;
    private const int SW_SHOWMINNOACTIVE = 7;
    private const int SW_SHOWNA = 8;
    private const int SW_RESTORE = 9;
    private const int SW_SHOWDEFAULT = 10;
    private const int SW_FORCEMINIMIZE = 11;
    private const int SW_MAX = 11;

    [DllImport("User32.Dll")]
    private static extern bool ShowWindow
    (
        IntPtr hWnd,
        int nCmdShow
    );

    [DllImport("User32.Dll")]
    private static extern bool CloseWindow
    (
        IntPtr hWnd
    );

    [DllImport("user32")]
    private static extern IntPtr GetDesktopWindow();

    #endregion
}
