namespace ToDoApp.Models
{
    public class IndexModel
    {
        public bool ShowComplete { get; set; }
        public List<TodoItem> TodoItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
