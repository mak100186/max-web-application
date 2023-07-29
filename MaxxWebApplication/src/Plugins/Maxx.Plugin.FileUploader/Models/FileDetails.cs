﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maxx.Plugin.FileUploader.Models;
[Table("FileDetails")]
public class FileDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public FileType FileType { get; set; }
}
