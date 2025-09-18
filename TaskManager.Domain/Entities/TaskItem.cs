namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; private set; }
    public bool IsCompleted { get; private set; }

    // Konstruktor: obavezna je opis/description
    public TaskItem(string description, DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        Description = description.Trim();
        DueDate = dueDate;
    }

    public void Complete() => IsCompleted = true;

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Description cannot be empty.", nameof(newDescription));
        Description = newDescription.Trim();
    }

    public void UpdateDueDate(DateTime? newDueDate) => DueDate = newDueDate;
}
