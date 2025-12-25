using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HMS.Application.Interfaces;
using HMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload/profile")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("No file uploaded"));
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!_fileService.IsValidFileType(file, allowedExtensions))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid file type. Only JPG, PNG, GIF allowed"));
            }

            // Validate file size (5MB)
            if (!_fileService.IsValidFileSize(file, 5))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("File size exceeds 5MB limit"));
            }

            var filePath = await _fileService.UploadFileAsync(file, "profiles");

            return Ok(ApiResponse<string>.SuccessResponse(filePath, "Profile picture uploaded successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.FailureResponse($"Upload failed: {ex.Message}"));
        }
    }

    [HttpPost("upload/document")]
    public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("No file uploaded"));
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            if (!_fileService.IsValidFileType(file, allowedExtensions))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid file type"));
            }

            // Validate file size (10MB)
            if (!_fileService.IsValidFileSize(file, 10))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("File size exceeds 10MB limit"));
            }

            var filePath = await _fileService.UploadFileAsync(file, "documents");

            return Ok(ApiResponse<string>.SuccessResponse(filePath, "Document uploaded successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.FailureResponse($"Upload failed: {ex.Message}"));
        }
    }

    [HttpPost("upload/labreport")]
    [Authorize(Roles = "Admin,LabTechnician")]
    public async Task<IActionResult> UploadLabReport([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("No file uploaded"));
            }

            var allowedExtensions = new[] { ".pdf" };
            if (!_fileService.IsValidFileType(file, allowedExtensions))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Only PDF files allowed"));
            }

            if (!_fileService.IsValidFileSize(file, 15))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("File size exceeds 15MB limit"));
            }

            var filePath = await _fileService.UploadFileAsync(file, "labreports");

            return Ok(ApiResponse<string>.SuccessResponse(filePath, "Lab report uploaded successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.FailureResponse($"Upload failed: {ex.Message}"));
        }
    }

    [HttpGet("download/{folder}/{filename}")]
    public async Task<IActionResult> DownloadFile(string folder, string filename)
    {
        try
        {
            var filePath = Path.Combine(folder, filename);
            var fileBytes = await _fileService.GetFileAsync(filePath);
            var contentType = GetContentType(filename);

            return File(fileBytes, contentType, filename);
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse($"File not found: {ex.Message}"));
        }
    }

    [HttpDelete("{folder}/{filename}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFile(string folder, string filename)
    {
        try
        {
            var filePath = Path.Combine(folder, filename);
            var deleted = await _fileService.DeleteFileAsync(filePath);

            if (deleted)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(true, "File deleted successfully"));
            }

            return NotFound(ApiResponse<bool>.FailureResponse("File not found"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.FailureResponse($"Delete failed: {ex.Message}"));
        }
    }

    private string GetContentType(string filename)
    {
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }
}