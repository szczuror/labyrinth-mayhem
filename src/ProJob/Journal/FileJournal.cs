using ProJob.Observer;

namespace ProJob.Journal;

public sealed class FileJournal : Observable<string>, IJournal
{
    private static FileJournal? _instance;
    private static readonly object _initLock = new();

    private readonly List<string> _entries = [];
    private readonly object _writeLock = new();
    private readonly string _filePath;

    private FileJournal(string filePath)
    {
        _filePath = filePath;
        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);
    }

    public static FileJournal Instance
    {
        get
        {
            if (_instance == null)
                throw new InvalidOperationException("FileJournal must be initialized before use. Call FileJournal.Initialize() first.");
            return _instance;
        }
    }

    public static void Initialize(string playerName, string savePath, DateTime startTime)
    {
        lock (_initLock)
        {
            if (_instance != null)
                return;

            string safePlayer = string.Concat(playerName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            string fileName = $"{safePlayer}_{startTime:yyyy-MM-dd_HH-mm-ss}.log";
            string fullPath = Path.Combine(savePath, "Logs", fileName);

            _instance = new FileJournal(fullPath);
        }
    }

    public string FilePath => _filePath;

    public void Log(string entry, int? playerId = null)
    {
        string prefix = playerId.HasValue ? $"[Gracz #{playerId.Value}] " : string.Empty;
        string timestamped = $"[{DateTime.Now:HH:mm:ss}] {prefix}{entry}";
        lock (_writeLock)
        {
            _entries.Add(timestamped);
            File.AppendAllText(_filePath, timestamped + Environment.NewLine);
        }
        Notify(timestamped);
    }

    public void Append(string formattedLine)
    {
        lock (_writeLock)
        {
            _entries.Add(formattedLine);
            File.AppendAllText(_filePath, formattedLine + Environment.NewLine);
        }
    }

    public IReadOnlyList<string> GetRecent(int count)
    {
        lock (_writeLock)
        {
            int start = Math.Max(0, _entries.Count - count);
            return _entries.GetRange(start, _entries.Count - start);
        }
    }

    public IReadOnlyList<string> GetAll()
    {
        lock (_writeLock)
        {
            return _entries.AsReadOnly();
        }
    }
}
