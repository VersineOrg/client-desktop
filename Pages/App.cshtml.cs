using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
        public class VoteFormat
        {
            public string? token { get; set; }
            public string? id { get; set; }
            public string? direction { get; set; }
        }   

    public class App : PageModel
    {
        public async Task OnGet(string timelineType)
        {
            using var client = new HttpClient();

            string username = (string) StorageManager.storage.Get("username");
            string baseurl = "https://api.versine.fr/users/profile";

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

            if (json != null && (string) json.status == "success") {
                string jsonuser = json.data;
                dynamic profile = JsonConvert.DeserializeObject(jsonuser)!;
                
                StorageManager.storage.Store("username", username);
                StorageManager.storage.Store("avatar", profile.avatar);
                StorageManager.storage.Store("bio", profile.bio);
                StorageManager.storage.Store("banner", profile.banner);
                StorageManager.storage.Store("color", profile.color);
                StorageManager.storage.Store("id", profile.id);
                StorageManager.storage.Store("ticket", profile.ticket);
                StorageManager.storage.Store("ticketCount", profile.ticketCount);


                string friendsString = profile.friends;
                bool reading = false;
                string currentId = "";
                List<string> friends = new List<string>();


                for (int i = 0; i < friendsString.Length; i++)
                {
                    if (reading)
                    {
                        if (friendsString[i] != '"')
                        {
                            currentId += friendsString[i];
                        }
                        else {
                            reading = false;
                            friends.Add(currentId);
                            currentId = "";
                        }
                    }
                    else {
                        if (friendsString[i] == '"')
                        {
                            reading = true;
                        }
                    }
                }

                StorageManager.storage.Store("friends", friends);

                using var client2 = new HttpClient();
                if (timelineType == null || (timelineType != "getTimeline") || (timelineType != "getCoolTimeline"))
                {
                    timelineType = "getTimeline";
                }
                string baseurl2 = "https://api.versine.fr/timeline/" + timelineType;

                ProfileFormat bodyObject2 = new ProfileFormat()
                {
                    token = token
                };

                
                string requestBody2 = JsonConvert.SerializeObject(bodyObject2);
                var httpContent2 = new StringContent(requestBody2, Encoding.UTF8, "application/json");
                var result2 = await client2.PostAsync(baseurl2, httpContent2);

                string bodyString2 = await result2.Content.ReadAsStringAsync();
                dynamic json2 = JsonConvert.DeserializeObject(bodyString2)!;

                dynamic timeline = new List<dynamic>();
                if (json2 != null && (string) json2.status == "success") {
                    string jsontimeline = json2.data;
                    timeline = JsonConvert.DeserializeObject(jsontimeline)!;
                    StorageManager.storage.Store("timeline", timeline);
                }

                StorageManager.storage.Store("timeline", timeline);
            }
            else
            {
                RedirectToPage("/Door");
            }
        }

        public async Task<ActionResult> OnPostVoteAsync(string data)
        {
            string[] actions = data.Split(',');
            string direction = actions[0];
            string postId = actions[1];

            using var client = new HttpClient();

            string username = (string) StorageManager.storage.Get("username");
            string baseurl = "https://api.versine.fr/posts/vote";

            string token = StorageManager.storage.Get("token").ToString();
            VoteFormat bodyObject = new VoteFormat()
            {
                token = token,
                id = postId,
                direction = direction
            };
            
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(baseurl, httpContent);

            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;

            if (json != null && (string) json.status == "success") {
                return RedirectToPage("/App");
            }

            return RedirectToPage("/App");
        }
    }
}
