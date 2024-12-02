using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPOCFlow
{
    public static class Helper
    {
        public static User UserNamePassWord()
        {
            var user = new User();  
            Console.Write("Enter username: ");
            user.UserName = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            user.Password = password;
            return user;    
        }
    }

    public record User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
