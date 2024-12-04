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

        public static async Task CallWebApiAndProcessResultASync(string? apiEndpoint, string accessToken)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                throw new ArgumentNullException(nameof(apiEndpoint), "API endpoint cannot be null or empty.");
            }

            using var httpClient = new HttpClient();
            var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }

            defaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine($"\nMaking HTTP Call to {apiEndpoint}");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint).ConfigureAwait(false);

                Console.WriteLine($"Response Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine($"Response Body: {responseBody}");
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine($"Error Body: {errorBody}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timed out: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }

        }

    }

    public record User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
