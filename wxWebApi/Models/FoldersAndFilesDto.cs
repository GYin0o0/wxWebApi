namespace wxWebApi.Models
{
    public class FoldersAndFilesDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? FilePath { get; set; }

        public int ParentId { get; set; }

        public int Type {  get; set; }

        public string? CreateTime { get; set; }

        public int FileType { get; set; }
    }
}
