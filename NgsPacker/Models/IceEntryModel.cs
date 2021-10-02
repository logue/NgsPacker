namespace NgsPacker.Models
{
    /// <summary>
    /// Iceをやり取りするときのモデル
    /// </summary>
    public class IceEntryModel
    {

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// データ
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// グループ
        /// </summary>
        public IceGroupEnum Group { get; set; }
    }
    public enum IceGroupEnum
    {
        GROUP1,
        GROUP2
    }
}
