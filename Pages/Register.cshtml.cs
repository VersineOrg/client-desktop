#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class RegisterFormat
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? ticket { get; set;}
    }
    public class RegisterModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;

        public RegisterModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        public string? Ticket { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {

            // "UserData" BindProperty (see above) 
            //  contains all the values entered by the user 

            // process them here 

            // . . . and redirect 
            Console.WriteLine(Username);
            Console.WriteLine(Password);
            Console.WriteLine(Ticket);
            
            using var client = new HttpClient();
            RegisterFormat bodyObject = new RegisterFormat()
            {
                username = Username,
                password = Password,
                ticket = Ticket
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            string baseurl = "http://localhost:8001";
            var result = await client.PostAsync(baseurl + "/register", httpContent);
            string? bodyString = await result.Content.ReadAsStringAsync();
            Console.WriteLine(bodyString);
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            
            StorageManager.storage.Store("token", json.data);
            return RedirectToPage(new {msg = "Success!"});

        }
        
        public void OnGet()
        {
            
        }
    }
}
