namespace Blizzard.IO.RabbitMQ.Builders
{
    internal struct ConnectionId
    {
        public string Hostname { get; }
        public string Username { get; }
        public string Password { get; }

        public ConnectionId(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
        }
    }
}
