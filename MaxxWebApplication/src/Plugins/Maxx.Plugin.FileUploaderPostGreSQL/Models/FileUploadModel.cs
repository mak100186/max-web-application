using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Models;

public class FileUploadModel
{
    [Required]
    public IFormFile FileDetails { get; set; }
    [Required]
    public FileType FileType { get; set; }
}
