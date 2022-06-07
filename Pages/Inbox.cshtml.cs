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
    
    public class InboxModel : PageModel
    {
        public async Task<RedirectToPageResult> OnPostAcceptFriendRequestAsync(string data)
        {
            using var client = new HttpClient();

            string baseurl = "https://api.versine.fr/users/requestFriend";
            string token = StorageManager.storage.Get("token").ToString();
            RequestFriendFormat bodyObject = new RequestFriendFormat()
            {
                token = token,
                requesteduserid = data
            };

            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(baseurl, httpContent);


            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;

            return RedirectToPage("/Inbox", new { message = "Success"});
        }

        public async Task OnGetAsync()
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

                string incomingFriendRequestsString = profile.incomingFriendRequests;
                bool reading = false;
                string currentId = "";
                List<string> friendRequestsList = new List<string>();


                for (int i = 0; i < incomingFriendRequestsString.Length; i++)
                {
                    if (reading)
                    {
                        if (incomingFriendRequestsString[i] != '"')
                        {
                            currentId += incomingFriendRequestsString[i];
                        }
                        else {
                            reading = false;
                            friendRequestsList.Add(currentId);
                            currentId = "";
                        }
                    }
                    else {
                        if (incomingFriendRequestsString[i] == '"')
                        {
                            reading = true;
                        }
                    }
                }
                StorageManager.storage.Store("incomingFriendRequests", friendRequestsList);
                Newtonsoft.Json.Linq.JArray incomingFriendRequests = (Newtonsoft.Json.Linq.JArray) StorageManager.storage.Get("incomingFriendRequests");
                List<dynamic> fullFriends = new List<dynamic>();

                foreach (dynamic friendId in incomingFriendRequests)
                {
                    using var client2 = new HttpClient();

                    string baseurl2 = "https://api.versine.fr/users/userById/" + (string) friendId;

                    ProfileFormat bodyObject2 = new ProfileFormat()
                    {
                        token = token
                    };
                    
                    string requestBody2 = JsonConvert.SerializeObject(bodyObject2);
                    var httpContent2 = new StringContent(requestBody2, Encoding.UTF8, "application/json");
                    var result2 = await client2.PostAsync(baseurl2, httpContent2);
                    string bodyString2 = await result2.Content.ReadAsStringAsync();
                    dynamic json2 = JsonConvert.DeserializeObject(bodyString2)!;
                    string jsonuser2 = json2.data;
                    if (jsonuser2 != null)
                    {
                        dynamic profile2 = JsonConvert.DeserializeObject(jsonuser2)!;
                        fullFriends.Add(profile2);
                    }
                }

                StorageManager.storage.Store("incomingFriendRequests", fullFriends);
            }
             
        }
        
    }
}
