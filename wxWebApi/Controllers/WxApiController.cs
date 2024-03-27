using Microsoft.AspNetCore.Mvc;
using wxWebApi.Models;
using wxWebApi.Utils;

namespace wxWebApi.Controllers
{
    public class WxApiController(WxFoldersContext context, WxFilesContext filesContext) : Controller
    {
        private readonly WxFoldersContext _context = context;

        private readonly WxFilesContext _filesContext = filesContext;

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// create folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpPost("createFolder")]
        public IActionResult CreateFolder([FromBody] WxFolders folder)
        {
            if (_context.WxFolders.FirstOrDefault(s => s.Name == folder.Name && s.Parent == folder.Parent) != null)
            {
                folder.Name += DateTime.Now.ToString("_yyyyMMdd_HHmmss");
            }

            folder.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _context.Add(folder);
            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// get folders
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        [HttpGet("getDatas")]
        public List<FoldersAndFilesDto> GetDatas(int parent)
        {
            List<FoldersAndFilesDto> dtos = [];

            List<WxFolders> wxFolders = [.. _context.WxFolders.Where(s => s.Parent == parent)];
            List<WxFiles> wxFiles = [.. _filesContext.WxFiles.Where(s => s.FolderId == parent)];

            wxFolders.ForEach(wxFolder =>
            {
                dtos.Add(new FoldersAndFilesDto()
                {
                    Id = wxFolder.Id,
                    Name = wxFolder.Name,
                    CreateTime = wxFolder.CreateTime,
                    ParentId = wxFolder.Parent,
                    Type = 0
                });
            });

            QiniuUtils qiniuUtils = init();
            wxFiles.ForEach(wxFile =>
            {
                dtos.Add(new FoldersAndFilesDto()
                {
                    Id = wxFile.Id,
                    Name = wxFile.FileName,
                    CreateTime = wxFile.CreateTime,
                    ParentId = wxFile.FolderId,
                    Type = 1,
                    FilePath = qiniuUtils.DownloadFile(wxFile.FilePath),
                    FileType = wxFile.FileType
                });
            });

            return [.. dtos.OrderByDescending(s => s.CreateTime)];
        }

        /// <summary>
        /// upload 
        /// </summary>
        /// <param name="file"></param>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int parent)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            WxFiles wxFiles = new()
            {
                FileName = file.FileName,
                FolderId = parent,
                FilePath = (GetFilePath(parent, "") + "/" + file.FileName)[1..],
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            QiniuUtils qiniuUtils = init();
            await qiniuUtils.UploadFileAsync(path, wxFiles.FilePath);

            _filesContext.Add(wxFiles);
            _filesContext.SaveChanges();

            return Ok(new { fileName = file.FileName, filePath = wxFiles.FilePath });
        }

        public string GetFilePath(int folderId, string path = "")
        {
            WxFolders? wxFolders = _context.WxFolders.FirstOrDefault(s => s.Id == folderId);
            if (wxFolders == null)
            {
                return path;
            }

            return GetFilePath(wxFolders.Parent, path + "/" + wxFolders.Name);
        }

        private QiniuUtils init()
        {
            return new("0g3S-S5hgpK04NRjJIc-BhIPTukky98J8y_L2TPV", "N8dHVFjd4Lu2AlhDOt1CYLBAOp11V2BQ0YQxxe3b", "yinguangkkoi");
        }
    }
}
