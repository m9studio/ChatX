namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server.Start(21235, "ChatX");

            string Command = "";
            string[] Commands;
            bool run = true;
            while (run)
            {
                Command = Console.ReadLine();
                if (Command != null)
                {
                    Command = Command.Trim().ToLower();
                    if(Command != "")
                    {
                        Commands = Command.Split(' ');
                        switch (Commands[0])
                        {
                            case "help":
                                Console.WriteLine("Chat -C - Return the count of chats");
                                Console.WriteLine("Chat -L - Return the list of chats");
                                Console.WriteLine("User -C - Return the count of users in all chats");
                                Console.WriteLine("User -C [id chat] - Return the count of users in the selected chat");
                                Console.WriteLine("stop - Stoping the server");
                                break;
                            case "stop":
                                Server.Stop();
                                Console.WriteLine("Stoped");
                                Console.ReadKey();
                                run = false;
                                break;
                            case "chat":
                                if(Commands.Length >= 2)
                                {
                                    switch(Commands[1])
                                    {
                                        case "-c":
                                            Console.WriteLine(Server.__CountChat());
                                            break;
                                        case "-l":
                                            Console.WriteLine(Server.__ListChat());
                                            break;
                                        default:
                                            Console.WriteLine("The command is not recognized");
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The command is not recognized");
                                }
                                break;
                            case "user":
                                if (Commands.Length >= 2 && Commands[1] == "-c")
                                {

                                    switch (Commands.Length)
                                    {
                                        case 2:
                                            Console.WriteLine(Server.__CountUser());
                                            break;
                                        case 3:
                                            int i = 0;
                                            if (int.TryParse(Commands[2], out i))
                                            {
                                                Console.WriteLine(Server.__CountChatUser(i));
                                            }
                                            else
                                            {
                                                Console.WriteLine("The command is not recognized");
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The command is not recognized");
                                }
                                break;
                            default:
                                Console.WriteLine("The command is not recognized");
                                break;
                        }
                    }
                }
            }
        }
    }
}