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
    public class LoginFormat
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {

            // "UserData" BindProperty (see above) 
            //  contains all the values entered by the user 

            // process them here 

            // . . . and redirect 
            Console.WriteLine(Username);
            Console.WriteLine(Password);
            
            using var client = new HttpClient();
            LoginFormat bodyObject = new LoginFormat()
            {
                username = Username,
                password = Password,
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://localhost:8001/login", httpContent);
            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            
            StorageManager.storage.Store("token", json.data);
            return RedirectToPage(new {msg = "Success!"});

        }
        public void OnGet()
        {
            
        }
    }
}
