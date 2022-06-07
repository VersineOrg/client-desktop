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
    public class DeleteUserFormat
    {
        public string? token { get; set; }
        public string? password { get; set; }
    }
    public class DeleteUserModel : PageModel
    {
        [BindProperty]
        public string? Password { get; set; }
       
        public async Task<ActionResult> OnPostDelete(string data)
        {
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");

            if (Password != null)
            {
                DeleteUserFormat bodyObject = new DeleteUserFormat()
                {
                    token = token,
                    password = Password
                };

                string requestBody = JsonConvert.SerializeObject(bodyObject);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                string baseurl = "http://api.versine.fr/users/deleteUser";
                var result = await client.PostAsync(baseurl, httpContent);
                string? bodyString = await result.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(bodyString)!;
                
                if ((string) json.status == "success")
                {
                    StorageManager.storage.Clear();
                    return RedirectToPage("/Index");
                }
                else {
                    return RedirectToPage("/DeleteUser");
                }
            }
            else {
                return RedirectToPage("/DeleteUser");
            }
        }

        public void OnGet()
        {
            
        }
    }
}
