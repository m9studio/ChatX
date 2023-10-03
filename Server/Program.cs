namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server.Start(21235, "ChatX");
            
            while(true)
            {
                Console.ReadLine();
            }
        }
    }
}