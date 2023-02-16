// -----------------------------------------------------------------------
// <copyright file="DwmApi.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace NgsPacker.Helpers;

/// <summary>
///     ディスクトップウィンドウマネージャーのP/invoke
/// </summary>
/// <see href="https://learn.microsoft.com/ja-jp/windows/win32/api/_dwm/" />
public class DwmApi
{
    /// <summary>
    ///     Enable Mica on the given HWND.
    /// </summary>
    /// <param name="source">対象のハンドル</param>
    /// <param name="darkThemeEnabled">ダークモードか</param>
    /// <see
    ///     href="https://github.com/MicrosoftDocs/windows-dev-docs/blob/docs/hub/apps/desktop/modernize/apply-rounded-corners.md" />
    public static void EnableMica(HwndSource source, bool darkThemeEnabled)
    {
        // ちゃんと動くけどたぶん記述が間違っている
        DWM_WINDOW_CORNER_PREFERENCE rounded = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DWM_WINDOW_CORNER_PREFERENCE normal = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;

        // Set dark mode before applying the material, otherwise you'll get an ugly flash when displaying the window.
        if (darkThemeEnabled)
        {
            DwmSetWindowAttribute(
                source.Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                ref rounded,
                sizeof(uint));
        }
        else
        {
            DwmSetWindowAttribute(
                source.Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                ref normal,
                sizeof(uint));
        }

        DwmSetWindowAttribute(
            source.Handle,
            DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref rounded,
            sizeof(uint));
    }

    /// <summary>
    ///     ウィンドウのデスクトップ ウィンドウ マネージャー (DWM) のクライアント以外のレンダリング属性の値を設定します。 プログラミング ガイダンスとコード例については、「
    ///     クライアント以外の領域のレンダリングの制御」を参照してください。
    /// </summary>
    /// <param name="hWnd">ハンドラ</param>
    /// <param name="dwAttribute">
    ///     <see cref="DWMWINDOWATTRIBUTE">DWMWINDOWATTRIBUTE</see> 列挙型の値として指定された、設定する値を示すフラグ。
    ///     このパラメーターは、設定する属性を指定し、 pvAttribute パラメーターは属性値を含むオブジェクトを指します。
    /// </param>
    /// <param name="pvAttribute">
    ///     設定する属性値を含むオブジェクトへのポインター。 値セットの型は 、dwAttribute パラメーターの値によって異なります。
    ///     <see cref="DWMWINDOWATTRIBUTE">DWMWINDOWATTRIBUTE</see>
    ///     列挙トピックは、各フラグの行で、pvAttribute パラメーターにポインターを渡す必要がある値の種類を示します。
    /// </param>
    /// <param name="cbAttribute">
    ///     pvAttribute パラメーターを使用して設定される属性値のサイズ (バイト単位)。 値セットの型、つまりサイズ (バイト単位) は、 dwAttribute
    ///     パラメーターの値によって異なります。
    /// </param>
    /// <see href="https://learn.microsoft.com/ja-jp/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute" />
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void DwmSetWindowAttribute(
        IntPtr hWnd,
        DWMWINDOWATTRIBUTE dwAttribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, // そもそもここの記述おかしくない？
        uint cbAttribute);

#pragma warning disable CA1707 // 識別子はアンダースコアを含むことはできません
    /// <summary>
    ///     DwmSetWindowAttribute 関数がウィンドウの角を丸く設定するために使用するフラグ。
    /// </summary>
    [Flags]
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        /// <summary>
        ///     ウィンドウの角を丸めるタイミングをシステムに決定させます。
        /// </summary>
        DWMWCP_DEFAULT = 0,

        /// <summary>
        ///     ウィンドウの角は丸められません。
        /// </summary>
        DWMWCP_DONOTROUND = 1,

        /// <summary>
        ///     必要に応じて、角を丸くします。
        /// </summary>
        DWMWCP_ROUND = 2,

        /// <summary>
        ///     必要に応じて角を丸め、半径を小さくします。
        /// </summary>
        DWMWCP_ROUNDSMALL = 3
    }

    /// <summary>
    ///     DwmGetWindowAttribute 関数と DwmSetWindowAttribute 関数で使用されるオプション。
    /// </summary>
    [Flags]
    public enum DWMWINDOWATTRIBUTE : uint
    {
        /// <summary>
        ///     DwmGetWindowAttribute で使用します。 クライアント以外のレンダリングが有効になっているかどうかを検出します。
        ///     取得された値は BOOL 型です。 クライアント以外のレンダリングが有効な場合は TRUE。それ以外の場合は FALSE。
        /// </summary>
        DWMWA_NCRENDERING_ENABLED = 1,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 クライアント以外のレンダリング ポリシーを設定します。
        ///     pvAttribute パラメーターは、DWMNCRENDERINGPOLICY 列挙型の値を指します。
        /// </summary>
        DWMWA_NCRENDERING_POLICY,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 DWM 遷移を有効または強制的に無効にします。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。 遷移 を無効にする場合は TRUE、切り替えを有効にする場合は FALSE 。
        /// </summary>
        DWMWA_TRANSITIONS_FORCEDISABLED,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 クライアント以外の領域にレンダリングされたコンテンツを DWM によって描画されたフレームに表示できるようにします。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。 TRUE を指定すると、クライアント以外の領域にレンダリングされたコンテンツがフレームに表示されます。
        ///     それ以外の場合は FALSE。
        /// </summary>
        DWMWA_ALLOW_NCPAINT,

        /// <summary>
        ///     DwmGetWindowAttribute で使用します。 ウィンドウ相対スペース内のキャプション ボタン領域の境界を取得します。
        ///     取得された値は RECT 型です。 ウィンドウが最小化されている場合、またはユーザーに表示されない場合、取得された RECT の値は未定義です。
        ///     取得した RECT に使用できる境界が含まれているかどうかを確認する必要があります。
        ///     使用できない場合は、ウィンドウが最小化されているか、それ以外の場合は表示されないと結論付けることができます。
        /// </summary>
        DWMWA_CAPTION_BUTTON_BOUNDS,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 クライアント以外のコンテンツが右から左 (RTL) にミラー化されているかどうかを指定します。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。 クライアント以外のコンテンツが右から左 (RTL) にミラー化されている場合は TRUE。それ以外の場合は FALSE。
        /// </summary>
        DWMWA_NONCLIENT_RTL_LAYOUT,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ウィンドウのライブまたはスナップショット表現が使用可能な場合でも、
        ///     アイコンのサムネイルまたはピーク表現 (静的ビットマップ) をウィンドウに強制的に表示します。
        ///     この値は通常、ウィンドウの作成時に設定され、ウィンドウの有効期間中は変更されません。
        ///     ただし、一部のシナリオでは、時間の経過と同時に値の変更が必要になる場合があります。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。 象徴的なサムネイルまたはピーク表現を要求する場合は TRUE。それ以外の場合は FALSE。
        /// </summary>
        DWMWA_FORCE_ICONIC_REPRESENTATION,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 Flip3D によるウィンドウの処理方法を設定します。
        ///     pvAttribute パラメーターは、DWMFLIP3DWINDOWPOLICY 列挙型の値を指します。
        /// </summary>
        DWMWA_FLIP3D_POLICY,

        /// <summary>
        ///     DwmGetWindowAttribute で使用します。 画面空間内の拡張フレーム境界の四角形を取得します。 取得された値は RECT 型です。
        /// </summary>
        DWMWA_EXTENDED_FRAME_BOUNDS,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。
        ///     ウィンドウは、DWM で使用するビットマップを、ウィンドウのアイコンサムネイルまたはピーク表現 (静的ビットマップ) として提供します。
        ///     DWMWA_HAS_ICONIC_BITMAP は 、DWMWA_FORCE_ICONIC_REPRESENTATIONで指定できます。
        ///     DWMWA_HAS_ICONIC_BITMAP は通常、ウィンドウの作成時に設定され、ウィンドウの有効期間中は変更されません。
        ///     ただし、一部のシナリオでは、時間の経過と同時に値の変更が必要になる場合があります。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。
        ///     ウィンドウが象徴的なサムネイルまたはピーク表現を提供することを DWM に通知する場合は TRUE。それ以外の場合は FALSE。
        /// </summary>
        DWMWA_HAS_ICONIC_BITMAP,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ウィンドウのプレビュープレビューは表示しないでください。
        ///     ピーク ビューには、タスク バーのウィンドウのサムネイルにマウス ポインターを合わせると、ウィンドウのフル サイズのプレビューが表示されます。
        ///     この属性が設定されている場合、ウィンドウのサムネイルの上にマウス ポインターを置くとピークが閉じます (グループ内の別のウィンドウにプレビューが表示されている場合)。
        ///     pvAttribute パラメーターは、BOOL 型の値を指します。 ピーク 機能を禁止する場合は TRUE、許可する場合 は FALSE 。
        /// </summary>
        DWMWA_DISALLOW_PEEK,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ピークが呼び出されたときにウィンドウがガラス シートにフェードするのを防ぎます。 pvAttribute パラメーターは、BOOL 型の値を指します。
        ///     別のウィンドウのピーク時にウィンドウがフェードするのを防ぐには TRUE、通常の動作の場合は FALSE。
        /// </summary>
        DWMWA_EXCLUDED_FROM_PEEK,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ユーザーに表示されないようにウィンドウをクロークします。 ウィンドウは引き続き DWM によって構成されます。
        ///     DirectComposition での使用: DWMWA_CLOAK フラグを使用して、レイヤー化された子ウィンドウに関連付けられている DirectComposition ビジュアルを使用して
        ///     ウィンドウのコンテンツの表現をアニメーション化するときに、階層化された子ウィンドウをクロークします。
        ///     この使用例の詳細については、「 階層化された子ウィンドウのビットマップをアニメーション化する方法」を参照してください。
        /// </summary>
        DWMWA_CLOAK,

        /// <summary>
        ///     DwmGetWindowAttribute で使用します。 ウィンドウがクロークされている場合は、その理由を説明する次のいずれかの値を指定します。
        ///     DWM_CLOAKED_APP (値0x0000001)。 ウィンドウは、その所有者アプリケーションによってクロークされました。
        ///     DWM_CLOAKED_SHELL (値0x0000002)。 窓はシェルによってクロークされました。
        ///     DWM_CLOAKED_INHERITED(値0x0000004)。 クローク値は、その所有者ウィンドウから継承されました。
        /// </summary>
        DWMWA_CLOAKED,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ウィンドウのサムネイル画像を現在のビジュアルで固定します。
        ///     ウィンドウの内容と一致するようにサムネイル画像に対してそれ以上ライブ更新を行わないでください。
        /// </summary>
        DWMWA_FREEZE_REPRESENTATION,

        /// <summary>
        ///     ?
        /// </summary>
        DWMWA_PASSIVE_UPDATE_MODE,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 UWP 以外のウィンドウでホスト背景ブラシを使用できるようにします。
        ///     このフラグが設定されている場合、 Windows::UI::Composition API を呼び出す Win32 アプリは、ホストの背景ブラシを使用して透過性効果を構築できます
        ///     ( Compositor.CreateHostBackdropBrush を参照)。 pvAttribute パラメーターは、BOOL 型の値を指します。
        ///     ウィンドウ のホスト 背景ブラシを有効にする場合は TRUE、無効にする場合は FALSE 。
        /// </summary>
        DWMWA_USE_HOSTBACKDROPBRUSH,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ダーク モード システム設定が有効になっている場合に、このウィンドウのウィンドウ フレームをダーク モードの色で描画できるようにします。
        ///     互換性の理由から、システム設定に関係なく、すべてのウィンドウが既定でライト モードになります。 pvAttribute パラメーターは、BOOL 型の値を指します。
        ///     ウィンドウ のダーク モードを優先する場合は TRUE、常にライト モードを使用する 場合は FALSE 。
        /// </summary>
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ウィンドウの角の丸みを帯びた設定を指定します。 pvAttribute パラメーターは、DWM_WINDOW_CORNER_PREFERENCE型の値を指します。
        /// </summary>
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 ウィンドウの境界線の色を指定します。 pvAttribute パラメーターは COLORREF 型の値を指します。
        ///     アプリは、ウィンドウのアクティブ化の変更など、状態の変化に応じて境界線の色を変更する役割を担います。
        /// </summary>
        DWMWA_BORDER_COLOR,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 キャプションの色を指定します。 pvAttribute パラメーターは COLORREF 型の値を指します。
        /// </summary>
        DWMWA_CAPTION_COLOR,

        /// <summary>
        ///     DwmSetWindowAttribute で使用します。 キャプション テキストの色を指定します。 pvAttribute パラメーターは COLORREF 型の値を指します。
        /// </summary>
        DWMWA_TEXT_COLOR,

        /// <summary>
        ///     DwmGetWindowAttribute で使用します。 DWM がこのウィンドウの周囲に描画する外側の境界線の幅を取得します。 値は、ウィンドウの DPI によって異なる場合があります。
        ///     pvAttribute パラメーターは、UINT 型の値を指します。
        /// </summary>
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,

        /// <summary>
        ///     重要。 この値は、プレリリース バージョンのWindows Insider Previewで使用できます。
        ///     DwmGetWindowAttribute または DwmSetWindowAttribute で使用します。
        ///     クライアント以外の領域の背後を含む、ウィンドウのシステム描画の背景マテリアルを取得または指定します。
        ///     pvAttribute パラメーターは、DWM_SYSTEMBACKDROP_TYPE型の値を指します。
        /// </summary>
        DWMWA_SYSTEMBACKDROP_TYPE,

        /// <summary>
        ///     検証目的で使用される、認識される DWMWINDOWATTRIBUTE の最大値。
        /// </summary>
        DWMWA_LAST
    }

    /// <summary>
    ///     クライアント以外の領域の背後を含む、ウィンドウのシステム描画の背景マテリアルを指定するためのフラグ。
    /// </summary>
    private enum DWM_SYSTEMBACKDROP_TYPE
    {
        /// <summary>
        ///     これが既定値です。 デスクトップ ウィンドウ マネージャー (DWM) が、このウィンドウのシステム描画の背景マテリアルを自動的に決定できるようにします。
        /// </summary>
        DWMSBT_AUTO,

        /// <summary>
        ///     システムの背景を描画しないでください。
        /// </summary>
        DWMSBT_NONE,

        /// <summary>
        ///     有効期間の長いウィンドウに対応する背景素材効果を描画します。
        /// </summary>
        DWMSBT_MAINWINDOW,

        /// <summary>
        ///     一時的なウィンドウに対応する背景マテリアル効果を描画します。
        /// </summary>
        DWMSBT_TRANSIENTWINDOW,

        /// <summary>
        ///     タブ付きのタイトル バーがあるウィンドウに対応する背景素材効果を描画します。
        /// </summary>
        DWMSBT_TABBEDWINDOW
    }
#pragma warning restore CA1707 // 識別子はアンダースコアを含むことはできません
}
