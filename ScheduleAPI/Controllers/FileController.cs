using Microsoft.AspNetCore.Mvc;

namespace ScheduleAPI.Controllers
{

    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        //// 📌 API 1: Lấy danh sách thư mục
        //[HttpGet("folders")]
        //public IActionResult GetFolders([FromQuery] string? basePath)
        //{
        //    try
        //    {
        //        string rootPath = string.IsNullOrEmpty(basePath) ? "uploads" : basePath;
        //        string fullPath = Path.Combine(_env.ContentRootPath, rootPath);

        //        if (!Directory.Exists(fullPath))
        //            return NotFound("Thư mục không tồn tại!");

        //        var folders = Directory.GetDirectories(fullPath)
        //                               .Select(dir => Path.GetFileName(dir))
        //                               .ToList();

        //        return Ok(folders);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi: {ex.Message}");
        //    }
        //}

        // 📌 API 2: Upload file vào thư mục đã chọn
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string folderPath)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            try
            {
                string saveFolderPath = Path.Combine(_env.ContentRootPath, folderPath);
                if (!Directory.Exists(saveFolderPath))
                    return NotFound("Thư mục không tồn tại!");

                string fullServerFilePath = Path.Combine(saveFolderPath, file.FileName);

                using FileStream fs = new(fullServerFilePath, FileMode.Create);
                await file.CopyToAsync(fs);

                return Ok(new { Message = "Upload thành công!", FilePath = fullServerFilePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi upload file: {ex.Message}");
            }
        }
    }

}
