using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class App : PageModel
    {
        public async Task OnGet()
        {
             using var client = new HttpClient();

             string username = (string) StorageManager.storage.Get("username");
             string baseurl = "https://api.versine.fr/users/user/" + username;
             var result = await client.GetAsync(baseurl);
             string bodyString = await result.Content.ReadAsStringAsync();
             dynamic json = JsonConvert.DeserializeObject(bodyString)!;
             string jsonuser = json.data;
             dynamic profile = JsonConvert.DeserializeObject(jsonuser)!;
             
             StorageManager.storage.Store("username", username);
             StorageManager.storage.Store("avatar", profile.avatar);
             StorageManager.storage.Store("bio", profile.bio);
             StorageManager.storage.Store("banner", profile.banner);
             StorageManager.storage.Store("color", profile.color);
             StorageManager.storage.Store("id", profile.id);
             
        }
    }
}
