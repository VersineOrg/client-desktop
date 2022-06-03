using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class ProfileModel : PageModel
    {
        public void OnPostRequestFriend(string data)
        {
            Console.WriteLine("COUCU LES GSN=====================================================================");
        }
        public void OnPostFollow(string data)
        {
            Console.WriteLine("COUCU LES GSN=====================================================================");
        }

        public async Task OnGet(string username)
        {
            using var client = new HttpClient();

            string baseurl = "https://api.versine.fr/users/user/" + username;
            var result = await client.GetAsync(baseurl);
            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            string jsonuser = json.data;
            if (jsonuser != null)
            {
                dynamic profile = JsonConvert.DeserializeObject(jsonuser)!;
                
                StorageManager.storage.Store("queryUsername", username);
                StorageManager.storage.Store("queryAvatar", profile.avatar);
                StorageManager.storage.Store("queryBio", profile.bio);
                StorageManager.storage.Store("queryBanner", profile.banner);
                StorageManager.storage.Store("queryColor", profile.color);
                StorageManager.storage.Store("queryId", profile.id);
            }
            else {
                StorageManager.storage.Store("queryUsername", "User not found");
                StorageManager.storage.Store("queryAvatar", "https://i.stack.imgur.com/34AD2.jpg");
                StorageManager.storage.Store("queryBio", "User not found");
                StorageManager.storage.Store("queryBanner", "https://i.imgur.com/7lJRtfB.png");
                StorageManager.storage.Store("queryColor", "#000000");
                StorageManager.storage.Store("queryId", "1234");
            }
        }
        
    }
}
