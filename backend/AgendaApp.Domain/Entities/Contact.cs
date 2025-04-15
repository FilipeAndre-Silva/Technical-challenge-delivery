using AgendaApp.Domain.Common;

namespace AgendaApp.Domain.Entities;

public class Contact : BaseEntity
{
    protected Contact() : base() { }

    public Contact(string name, string email, string phone, Guid createdBy)
        : base(createdBy)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    public void Update(string? name, string? email, string? phone, Guid updatedBy)
    {
        bool hasChanged = false;

        if (!string.IsNullOrEmpty(name) && Name != name)
        {
            Name = name;
            hasChanged = true;
        }

        if (!string.IsNullOrEmpty(email) && Email != email)
        {
            Email = email;
            hasChanged = true;
        }

        if (!string.IsNullOrEmpty(phone) && Phone != phone)
        {
            Phone = phone;
            hasChanged = true;
        }

        if (hasChanged)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}