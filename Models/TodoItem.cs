namespace ToDoApp.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Task { get; set; }
        public bool Complete { get; set; }
        public string Owner { get; set; }
        public string Category { get; set; }
        public string LabelColor { get; set; }
        public string UserId { get; set; }
    }
}
