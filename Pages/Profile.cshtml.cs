using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class ProfileFormat
    {
        public string? token { get; set; }
    }
    public class FriendTimeLineFormat
    {
        public string? token { get; set; }
        public string? friendId { get; set; }
    }
    public class RequestFriendFormat
    {

        public string? token { get; set; }
        public string? requesteduserid { get; set; }
    }

    public class ProfileModel : PageModel
    {
        public async void OnPostRequestFriend(string data)
        {
            using var client = new HttpClient();
            string baseurl = "https://api.versine.fr/users/requestFriend";
            string token = StorageManager.storage.Get("token").ToString();
            string userid = StorageManager.storage.Get("queryId").ToString();
            RequestFriendFormat bodyObject = new RequestFriendFormat()
            {
                token = token,
                requesteduserid = userid
            };

            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(baseurl, httpContent);


            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;

            RedirectToPage("/Profile", new { username = data });
        }
        public async void OnPostUnfriend(string data)
        {
            using var client = new HttpClient();
            string baseurl = "https://api.versine.fr/users/deleteRequest";
            string token = StorageManager.storage.Get("token").ToString();
            string userid = StorageManager.storage.Get("queryId").ToString();
            RequestFriendFormat bodyObject = new RequestFriendFormat()
            {
                token = token,
                requesteduserid = userid
            };

            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(baseurl, httpContent);

            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;

            RedirectToPage("/Profile", new { username = data });
        }

        public async Task OnGet(string username)
        { 
            using var client = new HttpClient();

            string baseurl = "https://api.versine.fr/users/user/" + username;

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
                
                StorageManager.storage.Store("queryUsername", username);
                StorageManager.storage.Store("queryAvatar", profile.avatar);
                StorageManager.storage.Store("queryBio", profile.bio);
                StorageManager.storage.Store("queryBanner", profile.banner);
                StorageManager.storage.Store("queryColor", profile.color);
                StorageManager.storage.Store("queryId", profile.id);

                using var client2 = new HttpClient();


                string baseurl2;
                FriendTimeLineFormat bodyObject2;
                if (username == (string) StorageManager.storage.Get("username"))
                {
                    baseurl2 = "https://api.versine.fr/timeline/getSelfPosts";
                    bodyObject2 = new FriendTimeLineFormat()
                    {
                        token = token
                    };
                }
                else
                {
                    baseurl2 = "https://api.versine.fr/timeline/getPostsfromFriend";
                    bodyObject2 = new FriendTimeLineFormat()
                    {
                        token = token,
                        friendId = profile.id
                    };
                }

                

                string requestBody2 = JsonConvert.SerializeObject(bodyObject2);
                var httpContent2 = new StringContent(requestBody2, Encoding.UTF8, "application/json");
                var result2 = await client2.PostAsync(baseurl2, httpContent2);

                string bodyString2 = await result2.Content.ReadAsStringAsync();
                Console.WriteLine(bodyString2);
                dynamic json2 = JsonConvert.DeserializeObject(bodyString2)!;

                if (json2 != null && (string) json2.status == "success") {
                    string jsontimeline = json2.data;
                    dynamic timeline = JsonConvert.DeserializeObject(jsontimeline)!;
                    StorageManager.storage.Store("queryTimeline", timeline);
                }
                else{
                    dynamic timeline = new List<dynamic>();
                    StorageManager.storage.Store("queryTimeline", timeline);
                }
            }
            else {
                StorageManager.storage.Store("queryUsername", "User not found");
                StorageManager.storage.Store("queryAvatar", "https://i.stack.imgur.com/34AD2.jpg");
                StorageManager.storage.Store("queryBio", "User not found");
                StorageManager.storage.Store("queryBanner", "https://i.imgur.com/7lJRtfB.png");
                StorageManager.storage.Store("queryColor", "#000000");
                StorageManager.storage.Store("queryId", "1234");
                dynamic timeline = new List<dynamic>();
                StorageManager.storage.Store("queryTimeline", timeline);
            }
            
        }
        
    }
}
