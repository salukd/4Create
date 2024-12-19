namespace Clinica.Contracts.Trials;

public record GetTrialResponse(string TrialId, 
    string Title, 
    DateTime StartDate, 
    DateTime? EndDate, 
    int Participants, 
    string Status, 
    int DurationInDays);