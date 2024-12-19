using ErrorOr;

namespace Clinica.Application.Trials.Commands.ProcessFile;

public static class ProcessFileCommandErrors
{
    public static readonly Error CannotDeserializeContent = Error.Validation(
        "Trial.DeserializationError", 
        "Failed to deserialize JSON content.");

    public static readonly Error DuplicateTrialId = Error.Validation(
        "Trial.DuplicateTrialId", 
        "A trial with the same ID already exists.");

    public static readonly Error DatabaseSaveFailed = Error.Failure(
        "Trial.DatabaseSaveError", 
        "Failed to save trial metadata to the database.");
}