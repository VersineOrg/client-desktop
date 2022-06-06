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
    
    public class MessagesModel : PageModel
    {
        public async Task OnGetAsync()
        {

            Newtonsoft.Json.Linq.JArray friends = (Newtonsoft.Json.Linq.JArray) StorageManager.storage.Get("friends");
            List<dynamic> fullFriends = new List<dynamic>();

            foreach (dynamic friendId in friends)
            {
                using var client = new HttpClient();

                string baseurl = "https://api.versine.fr/users/userById/" + (string) friendId;

                string token = StorageManager.storage.Get("token").ToString();
                ProfileFormat bodyObject = new ProfileFormat()
                {
                    token = token
                };
                
                string requestBody = JsonConvert.SerializeObject(bodyObject);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(baseurl, httpContent);
                string bodyString = await result.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(bodyString)!;
                string jsonuser = json.data;
                if (jsonuser != null)
                {
                    dynamic profile = JsonConvert.DeserializeObject(jsonuser)!;
                    fullFriends.Add(profile);
                }
            }

            foreach (dynamic friend in fullFriends)
            {
                Console.WriteLine(friend);
            }
            
        }
        
    }
}
