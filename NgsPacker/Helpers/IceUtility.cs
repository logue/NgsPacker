using NgsPacker.Properties;
using System.IO;
using System.Text.RegularExpressions;

namespace NgsPacker.Helpers;

/// <summary>
///     Iceファイルに関するユーティリティ関数群
/// </summary>
public static class IceUtility
{
    /// <summary>
    ///     ICEファイルであるか
    /// </summary>
    /// <param name="buffer">バイト配列</param>
    /// <returns>ICEがマジックコードに含まれていたらtrue</returns>
    public static bool IsIceFile(byte[] buffer)
    {
        return !(buffer.Length <= 127 || buffer[0] != 73 || buffer[1] != 67 || buffer[2] != 69 || buffer[3] != 0);
    }

    /// <summary>
    ///     データファイルのある場所へのパス
    /// </summary>
    /// <returns>dataディレクトリのパス</returns>
    public static string GetDataDir()
    {
        if (!File.Exists(Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "pso2.exe"))
        {
            throw new FileNotFoundException("pso2.exe does not found. Are the pso2 directory settings correct?");
        }

        return Settings.Default.Pso2BinPath + Path.DirectorySeparatorChar + "data" +
               Path.DirectorySeparatorChar;
    }

    /// <summary>
    ///     対象のパスかの判定（後日、このクラスから独立させる予定）
    /// </summary>
    /// <param name="path">対象パス</param>
    /// <param name="target">対象ディレクトリ</param>
    /// <returns>対象だった場合true、そうでない場合false。licenseディレクトリは常にfalse</returns>
    public static bool IsTargetPath(string path, DataDirectoryType target = DataDirectoryType.Ngs)
    {
        if (path.Contains('.'))
        {
            // 入力パスがディレクトリの場合false
            return false;
        }

        switch (target)
        {
            case DataDirectoryType.Pso:
                // PSO2ディレクトリのみの場合
                return Regex.IsMatch(path, "win32" + Path.DirectorySeparatorChar) ||
                       Regex.IsMatch(path, "win32_na" + Path.DirectorySeparatorChar);
            case DataDirectoryType.Ngs:
                // NGSディレクトリのみの場合
                return Regex.IsMatch(path, "win32reboot" + Path.DirectorySeparatorChar) ||
                       Regex.IsMatch(path, "win32reboot_na" + Path.DirectorySeparatorChar);
            default:
            case DataDirectoryType.All:
                // すべて対象にする場合
                break;
        }

        // ライセンスディレクトリは対象外
        return !Regex.IsMatch(path, "license");
    }

    /// <summary>
    ///     入力パスからdataディレクトリまでのパスを抜いた値を返す（win32_reboot\00\112da290e9b607c4ab330e4a9103e1）
    /// </summary>
    /// <param name="inputPath">入力パス</param>
    /// <returns>エントリ名</returns>
    public static string GetEntryName(string inputPath)
    {
        return inputPath.Replace(GetDataDir(), string.Empty);
    }
}
