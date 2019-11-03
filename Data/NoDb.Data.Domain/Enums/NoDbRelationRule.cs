namespace NoDb.Data.Domain.Enums
{
    public enum NoDbRelationRule : byte
    {
        NoAction = 0,
        Cascade,
        SetNull,
        SetDefault
    }
}
