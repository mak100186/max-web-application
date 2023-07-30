using Maxx.Plugin.TodoListPostGreSQL.Models;

namespace Maxx.Plugin.TodoListPostGreSQL.Services;
public interface ITodosService
{
    public Task PostTodoAsync(string title, TodosType type);

    public Task PostMultiTodosAsync(List<TodosModel> todosData);

    public Task<List<TodosModel>> GetAll();
}

public class TodosService : ITodosService
{
    private readonly DbContextClass _dbContextClass;

    public TodosService(DbContextClass dbContextClass)
    {
        _dbContextClass = dbContextClass;
    }

    public async Task PostTodoAsync(string title, TodosType type)
    {
        try
        {
            var fileDetails = new TodoDetails()
            {
                Id = 0,
                TodoTitle = title,
                TodoType = type.ToString(),
            };

            var result = _dbContextClass.TodosDetails.Add(fileDetails);
            await _dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PostMultiTodosAsync(List<TodosModel> todoData)
    {
        try
        {
            foreach (var model in todoData)
            {
                var fileDetails = new TodoDetails()
                {
                    Id = 0,
                    TodoTitle = model.TodoTitle,
                    TodoType = model.TodoType.ToString(),
                };

                var result = _dbContextClass.TodosDetails.Add(fileDetails);
            }
            await _dbContextClass.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<TodosModel>> GetAll()
    {
        try
        {
            return _dbContextClass.TodosDetails
                .Select(x => new TodosModel() { TodoTitle = x.TodoTitle, TodoType = Enum.Parse<TodosType>(x.TodoType) })
                .ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
