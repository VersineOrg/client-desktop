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
            using var client = new HttpClient();
            LoginFormat bodyObject = new LoginFormat()
            {
                username = Username,
                password = Password,
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://api.versine.fr/door/login", httpContent);
            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            if ((string) json.status == "success")
            {
                StorageManager.storage.Store("token", json.data);
                StorageManager.storage.Store("username", Username);
                        
                return RedirectToPage("/App", new { msg = "Success" });    
            }
            else
            {
                return RedirectToPage("/Door", new { msg = "Invalid Credentials" });
            }

        }
        public void OnGet()
        {
            
        }
    }
}
