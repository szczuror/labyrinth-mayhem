namespace ProJob.Journal;

public interface IJournal
{
    void Log(string entry, int? playerId = null);
    void Append(string formattedLine);
    IReadOnlyList<string> GetRecent(int count);
    IReadOnlyList<string> GetAll();
    string FilePath { get; }
}
