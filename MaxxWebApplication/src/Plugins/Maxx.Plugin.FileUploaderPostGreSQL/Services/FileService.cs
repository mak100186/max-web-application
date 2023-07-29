using Maxx.Plugin.FileUploaderPostGreSQL.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Services;
public interface IFileService
{
    public Task PostFileAsync(IFormFile fileData, FileType fileType);

    public Task PostMultiFileAsync(List<FileUploadModel> fileData);

    public Task DownloadFileById(int fileName);
}

public class FileService : IFileService
{
    private readonly DbContextClass _dbContextClass;

    public FileService(DbContextClass dbContextClass)
    {
        _dbContextClass = dbContextClass;
    }

    public async Task PostFileAsync(IFormFile fileData, FileType fileType)
    {
        try
        {
            var fileDetails = new FileDetails()
            {
                Id = 0,
                FileName = fileData.FileName,
                FileType = fileType,
            };

            using (var stream = new MemoryStream())
            {
                await fileData.CopyToAsync(stream);
                fileDetails.FileData = stream.ToArray();
            }

            var result = _dbContextClass.FileDetails.Add(fileDetails);
            await _dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        try
        {
            foreach (var file in fileData)
            {
                var fileDetails = new FileDetails()
                {
                    Id = 0,
                    FileName = file.FileDetails.FileName,
                    FileType = file.FileType,
                };

                using (var stream = new MemoryStream())
                {
                    await file.FileDetails.CopyToAsync(stream);
                    fileDetails.FileData = stream.ToArray();
                }

                var result = _dbContextClass.FileDetails.Add(fileDetails);
            }
            await _dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DownloadFileById(int Id)
    {
        try
        {
            var file = _dbContextClass.FileDetails.Where(x => x.Id == Id).FirstOrDefaultAsync();

            var content = new MemoryStream(file.Result.FileData);
            var path = Path.Combine(
               Directory.GetCurrentDirectory(), "../../FileDownloaded",
               file.Result.FileName);

            await CopyStream(content, path);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task CopyStream(Stream stream, string downloadPath)
    {
        await using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
    }
}
