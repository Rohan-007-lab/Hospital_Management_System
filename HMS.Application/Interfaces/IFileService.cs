using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HMS.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<byte[]> GetFileAsync(string filePath);
    bool IsValidFileType(IFormFile file, string[] allowedExtensions);
    bool IsValidFileSize(IFormFile file, long maxSizeInMB);
}