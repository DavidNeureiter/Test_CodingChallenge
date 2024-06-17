namespace websLINE.Database
{
    public interface IDbObject
    {
        int Id { get; set; }
        bool Deleted { get; set; }
    }
}
