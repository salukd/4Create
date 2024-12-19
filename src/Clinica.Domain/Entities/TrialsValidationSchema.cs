namespace Clinica.Domain.Entities;

public class TrialsValidationSchema
{
    public int Id { get; set; }
    public required string ValidationSchema { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}