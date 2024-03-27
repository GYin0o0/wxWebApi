namespace wxWebApi.Models
{
    public class WxFolders
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? CreateTime { get; set; }

        public int Parent { get; set; }
    }
}
