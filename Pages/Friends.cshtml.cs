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
    
    public class FriendsModel : PageModel
    {
        public async Task<Microsoft.AspNetCore.Mvc.RedirectToPageResult> OnPostUnfriendAsync(string data)
        {
            using var client = new HttpClient();
            string baseurl = "https://api.versine.fr/users/deleteRequest";
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

            return RedirectToPage("/Friends");
        }
        public async Task OnGetAsync()
        {
                using var client1 = new HttpClient();

                string username = (string) StorageManager.storage.Get("username");
                string baseurl1 = "https://api.versine.fr/users/profile";

                string token = StorageManager.storage.Get("token").ToString();
                ProfileFormat bodyObject1 = new ProfileFormat()
                {
                    token = token
                };

                
                string requestBody1 = JsonConvert.SerializeObject(bodyObject1);
                var httpContent1 = new StringContent(requestBody1, Encoding.UTF8, "application/json");
                var result1 = await client1.PostAsync(baseurl1, httpContent1);

                string bodyString1 = await result1.Content.ReadAsStringAsync();
                dynamic json1 = JsonConvert.DeserializeObject(bodyString1)!;

                if (json1 != null && (string) json1.status == "success") {
                    string jsonuser1 = json1.data;
                    dynamic profile1 = JsonConvert.DeserializeObject(jsonuser1)!;
                    
                    string friendsString = profile1.friends;
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


                    Newtonsoft.Json.Linq.JArray friends1 = (Newtonsoft.Json.Linq.JArray) StorageManager.storage.Get("friends");
                    List<dynamic> fullFriends = new List<dynamic>();

                    foreach (dynamic friendId in friends1)
                    {
                        using var client = new HttpClient();

                        string baseurl = "https://api.versine.fr/users/userById/" + (string) friendId;

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

                    StorageManager.storage.Store("fullFriends", fullFriends);
                }
                else
                {
                    RedirectToPage("/App");
                }
        }
        
    }
}
