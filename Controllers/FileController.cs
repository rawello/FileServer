using FileServerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{filename}")]
        [Authorize]
        public async Task<IActionResult> GetFile(string filename)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var fileBytes = await _fileService.GetFileAsync(filename, userId, userRole);
            if (fileBytes != null)
            {
                return File(fileBytes, "application/octet-stream", filename);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim is missing" });
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid User ID format" });
            }

            if (await _fileService.UploadFileAsync(file, userId))
            {
                return Ok(new { message = "File uploaded successfully" });
            }
            return BadRequest(new { message = "File upload failed" });
        }

        [HttpDelete("{filename}")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(string filename)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (await _fileService.DeleteFileAsync(filename, userId, userRole))
            {
                return Ok(new { message = "File deleted successfully" });
            }
            return NotFound();
        }

        [HttpGet("userfiles")]
        [Authorize]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var files = await _fileService.GetUserFilesAsync(userId);
            return Ok(files);
        }
    }
}