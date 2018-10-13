namespace BlazorFourInARowFunctions.Models
{
    public enum GameActionStatuses
    {
        New = 201,
        AwaitingValidation = 102,
        Valid = 202,
        InvalidTooSoon = 429,
        InvalidColumnFull = 409
    }
}
