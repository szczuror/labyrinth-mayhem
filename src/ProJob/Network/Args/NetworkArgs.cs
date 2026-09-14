namespace ProJob.Network.Args;

public sealed class NetworkArgs
{
    public NetworkMode Mode { get; init; } = NetworkMode.Local;
    public int Port { get; init; } = 5555;
    public string Host { get; init; } = "127.0.0.1";

    public static NetworkArgs Parse(string[] args)
    {
        int serverIdx = Array.IndexOf(args, "--server");
        if (serverIdx >= 0)
        {
            int port = 5555;
            if (serverIdx + 1 < args.Length && int.TryParse(args[serverIdx + 1], out int p))
                port = p;
            return new NetworkArgs { Mode = NetworkMode.Server, Port = port };
        }

        int clientIdx = Array.IndexOf(args, "--client");
        if (clientIdx >= 0)
        {
            string host = "127.0.0.1";
            int port = 5555;
            if (clientIdx + 1 < args.Length)
            {
                string[] parts = args[clientIdx + 1].Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int p))
                {
                    host = parts[0];
                    port = p;
                }
            }
            return new NetworkArgs { Mode = NetworkMode.Client, Host = host, Port = port };
        }

        return new NetworkArgs { Mode = NetworkMode.Local };
    }

    public static NetworkArgs PromptUser()
    {
        Console.Write("Uruchomić jako (S)erwer czy (K)lient? [Enter = lokalnie]: ");
        string? choice = Console.ReadLine()?.Trim().ToUpperInvariant();

        if (choice == "S")
        {
            Console.Write("Port serwera [domyślnie 5555]: ");
            string? portStr = Console.ReadLine()?.Trim();
            int port = 5555;
            if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out int p))
                port = p;
            return new NetworkArgs { Mode = NetworkMode.Server, Port = port };
        }

        if (choice == "K")
        {
            Console.Write("Adres serwera [domyślnie 127.0.0.1:5555]: ");
            string? addr = Console.ReadLine()?.Trim();
            string host = "127.0.0.1";
            int port = 5555;
            if (!string.IsNullOrEmpty(addr))
            {
                string[] parts = addr.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int p))
                {
                    host = parts[0];
                    port = p;
                }
            }
            return new NetworkArgs { Mode = NetworkMode.Client, Host = host, Port = port };
        }

        return new NetworkArgs { Mode = NetworkMode.Local };
    }
}
