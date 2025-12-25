using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HMS.Application.Services;

public class FileService : IFileService
{
    private readonly string _uploadPath;

    public FileService()
    {
        // Set upload path (create this folder in HMS.API)
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            // Create folder if not exists
            var folderPath = Path.Combine(_uploadPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path
            return Path.Combine(folder, uniqueFileName);
        }
        catch (Exception ex)
        {
            throw new Exception($"File upload failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);

            if (!File.Exists(fullPath))
            {
                throw new Exception("File not found");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex)
        {
            throw new Exception($"File retrieval failed: {ex.Message}");
        }
    }

    public bool IsValidFileType(IFormFile file, string[] allowedExtensions)
    {
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(fileExtension);
    }

    public bool IsValidFileSize(IFormFile file, long maxSizeInMB)
    {
        var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        return file.Length <= maxSizeInBytes;
    }
}