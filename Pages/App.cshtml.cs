using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class ProfileFormat
    { public string? token { get; set; }
    }
    public class App : PageModel
    {
        public async Task OnGet()
        {
             using var client = new HttpClient();
             string token = (string) StorageManager.storage.Get("token");
             ProfileFormat bodyObject = new ProfileFormat()
             {
                 token = token 
             };
             string requestBody = JsonConvert.SerializeObject(bodyObject);
             var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
             string username = (string) StorageManager.storage.Get("username");
             string baseurl = "http://localhost:4000/profile/" + username;
             var result = await client.PostAsync(baseurl, httpContent);
             string bodyString = await result.Content.ReadAsStringAsync();
             dynamic json = JsonConvert.DeserializeObject(bodyString)!;
             string jsonuser = json.data;
             dynamic profile = JsonConvert.DeserializeObject(jsonuser)!;
             
             StorageManager.storage.Store("username", profile.user.username);
             StorageManager.storage.Store("avatar", profile.user.avatar);
             StorageManager.storage.Store("bio", profile.user.bio);
             StorageManager.storage.Store("banner", profile.user.banner);
             StorageManager.storage.Store("color", profile.user.color);
             StorageManager.storage.Store("id", profile.user.id);
             
        }
    }
}