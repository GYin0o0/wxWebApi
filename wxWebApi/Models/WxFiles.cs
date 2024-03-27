namespace wxWebApi.Models
{
    public class WxFiles
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public int FolderId { get; set; }

        public int FileType { get; set; }

        public string? CreateTime { get; set; }
    }
}
