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
    
    public class EditUserFormat
    {
        public string? token { get; set; }
        public string? bio { get; set; }
        public string? color { get; set; }
        public string? avatar { get; set; }
        public string? banner { get; set; }
    }
    public class EditUserModel : PageModel
    {
        [BindProperty]
        public string? Avatar { get; set; }

        [BindProperty]
        public string? Banner { get; set; }

        [BindProperty]
        public string? Bio { get; set; }

        public async Task<ActionResult> OnPostEdit(string data)
        {
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");

            string Color = (string) StorageManager.storage.Get("color");
            Bio = (Bio != null) ? Bio : (string) StorageManager.storage.Get("bio");
            Avatar = (Avatar != null) ? Avatar : (string) StorageManager.storage.Get("avatar");
            Banner = (Banner != null) ? Banner : (string) StorageManager.storage.Get("banner");

            EditUserFormat bodyObject = new EditUserFormat()
            {
                token = token,
                bio = Bio,
                avatar = Avatar,
                color = Color,
                banner = Banner
            };

            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            string baseurl = "http://api.versine.fr/users/editUser";
            var result = await client.PostAsync(baseurl, httpContent);
            string? bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            
            if ((string) json.status == "success")
            {
                StorageManager.storage.Store("bio", Bio);
                StorageManager.storage.Store("banner", Banner);
                StorageManager.storage.Store("Avatar", Avatar);
                return RedirectToPage("/Settings");
            }
            else {
                return RedirectToPage("/EditUser");
            }
        }
        public void OnGet()
        {
            
        }
    }
}
