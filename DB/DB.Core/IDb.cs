namespace DB.Core
{
    public interface IDb
    {
        string Execute(string input);
    }
}