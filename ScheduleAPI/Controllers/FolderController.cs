using Microsoft.AspNetCore.Mvc;

namespace ScheduleAPI.Controllers
{ 

    [Route("api/folders")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FolderController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // 📌 API Lấy danh sách thư mục dạng cây
        [HttpGet("tree")]
        public IActionResult GetFolderTree([FromQuery] string? basePath)
        {
            try
            {
                string rootPath = string.IsNullOrEmpty(basePath) ? "uploads" : basePath;
                string fullPath = Path.Combine(_env.ContentRootPath, rootPath);

                if (!Directory.Exists(fullPath))
                    return NotFound("Thư mục không tồn tại!");

                var tree = GetFolderStructure(fullPath);
                return Ok(tree);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        // 📌 Hàm đệ quy lấy cây thư mục
        private FolderNode GetFolderStructure(string path)
        {
            var node = new FolderNode
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                SubFolders = Directory.GetDirectories(path)
                                      .Select(subDir => GetFolderStructure(subDir))
                                      .ToList()
            };
            return node;
        }
    }

    // 📌 Model cây thư mục
    public class FolderNode
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public List<FolderNode> SubFolders { get; set; } = new();
    }

}
