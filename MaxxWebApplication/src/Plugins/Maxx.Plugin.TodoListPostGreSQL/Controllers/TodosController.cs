using Maxx.Plugin.TodoListPostGreSQL.Models;
using Maxx.Plugin.TodoListPostGreSQL.Services;

using Microsoft.AspNetCore.Mvc;

namespace Maxx.Plugin.TodoListPostGreSQL.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodosService _uploadService;

    public TodosController(ITodosService uploadService)
    {
        _uploadService = uploadService;
    }

    /// <summary>
    /// Single File Upload
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("PostSingleTodo")]
    public async Task<ActionResult> PostSingleFile([FromBody] TodosModel todoDetails)
    {
        if (todoDetails == null)
        {
            return BadRequest();
        }

        await _uploadService.PostTodoAsync(todoDetails.TodoTitle, todoDetails.TodoType);
        return Ok();
    }

    /// <summary>
    /// Create multiple todos
    /// </summary>
    /// <returns></returns>
    [HttpPost("PostMultipleTodo")]
    public async Task<ActionResult> PostMultipleFile([FromBody] List<TodosModel> todoDetails)
    {
        if (todoDetails == null)
        {
            return BadRequest();
        }

        await _uploadService.PostMultiTodosAsync(todoDetails);
        return Ok();
    }

    /// <summary>
    /// Gets all todos in the database
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<ActionResult> GetAll()
    {
        await _uploadService.GetAll();
        return Ok();
    }
}
