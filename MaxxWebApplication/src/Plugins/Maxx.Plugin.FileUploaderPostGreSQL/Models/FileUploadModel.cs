using Microsoft.AspNetCore.Http;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Models;

public class FileUploadModel
{
    public IFormFile FileDetails { get; set; }
    public FileType FileType { get; set; }
}
