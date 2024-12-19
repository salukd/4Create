namespace Clinica.Domain.Entities;

public class TrialMetadata
{
    public required string TrialId { get; set; }
    public required string Title { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Participants { get; set; }
    public required TrialStatus Status { get; set; }
    public int DurationInDays { get; set; }
    public void CalculateDuration()
    {
        if (!EndDate.HasValue && Status == TrialStatus.Ongoing)
        {
            EndDate = StartDate.AddMonths(1);
        }

        DurationInDays = EndDate >= StartDate ? (EndDate.Value - StartDate).Days : 0;
    }
}