using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Models;
[Table("filedetails")]
public class FileDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public FileType FileType { get; set; }
}
