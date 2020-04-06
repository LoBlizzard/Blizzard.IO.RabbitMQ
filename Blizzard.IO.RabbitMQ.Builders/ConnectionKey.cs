namespace Blizzard.IO.RabbitMQ.Builders
{
    internal struct ConnectionKey
    {
        public string Hostname { get; }
        public string Username { get; }
        public string Password { get; }

        public ConnectionKey(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
        }
    }
}
