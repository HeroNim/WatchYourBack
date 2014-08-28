namespace WatchYourBackLibrary
{
    public enum ServerCommands
    {
        SendLevels = 100,
        Start = 101,
        Pause = 102,
        Connect = 103,
        Disconnect = 104,
        Win = 105,
        Lose = 106,
        Tie = 107
    }

    public enum ServerSettings
    {
        TimeStep = 60,
        MaxConnections = 1,
        Port = 14242,
        TimeOut = 5
    }
}