using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maxx.Plugin.TodoListPostGreSQL.Models;

[Table(nameof(TodoDetails))]
public class TodoDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string TodoTitle { get; set; }
    public string TodoType { get; set; }
}
