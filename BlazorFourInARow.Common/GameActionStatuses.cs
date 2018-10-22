namespace BlazorFourInARow.Common
{
    public enum GameActionStatuses
    {
        AwaitingValidation = 102,
        Valid = 202,
        InvalidTooSoon = 429,
        InvalidColumnFull = 409,
        InvalidGameHasEnded = 410
    }
}
