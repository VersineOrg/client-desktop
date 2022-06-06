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
    public class AddToCircleFormat
    {
        public string? token { get; set; }
        public string? friendId { get; set; }
        public string? circleName { get; set; }
    }   
    public class RemoveFromCircleFormat
    {
        public string? token { get; set; }
        public string? friendId { get; set; }
        public string? circleName { get; set; }
    } 
    public class DeleteCircleFormat
    {
        public string? token { get; set; }
        public string? circleName { get; set; }
    } 
    public class ManageCircleModel : PageModel
    {

        public async Task OnGetAsync(string name)
        {
            StorageManager.storage.Store("currentCircleName", name);
            using var client1 = new HttpClient();

            string username = (string) StorageManager.storage.Get("username");
            string baseurl1 = "https://api.versine.fr/circles/userCircles";

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
                string jsoncircles = json1.data;
                dynamic circles1 = JsonConvert.DeserializeObject(jsoncircles)!;

                StorageManager.storage.Store("circles", circles1);
            }
            else {
                RedirectToPage("/App");
            }

            Newtonsoft.Json.Linq.JArray circles = (Newtonsoft.Json.Linq.JArray) StorageManager.storage.Get("circles");

            Newtonsoft.Json.Linq.JArray friends = (Newtonsoft.Json.Linq.JArray) StorageManager.storage.Get("friends");
            List<dynamic> fullFriends = new List<dynamic>();

            foreach (dynamic friendId in friends)
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

            
            List<dynamic> circleUsers = new List<dynamic>();

            foreach(dynamic circle in circles)
            {
                if (circle.name == name)
                {
                    foreach (string userId in circle.users)
                    {
                        using var client = new HttpClient();

                        string baseurl = "https://api.versine.fr/users/userById/" + userId;

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
                            circleUsers.Add(profile);
                        }
                    }
                }
            }

            List<dynamic> remainingUsers = new List<dynamic>();

            foreach (dynamic friend in fullFriends)
            {
                bool found = false;
                foreach (dynamic circleFriend in circleUsers)
                {
                    if (circleFriend.id == friend.id)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    remainingUsers.Add(friend);
                }
            }



            StorageManager.storage.Store("remainingUsers", remainingUsers);
            StorageManager.storage.Store("circleUsers", circleUsers);
        }

        public async Task<ActionResult> OnPostRemoveFromCircleAsync(string data)
        {
            string circleName = (string) StorageManager.storage.Get("currentCircleName");
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");
            RemoveFromCircleFormat bodyObject = new RemoveFromCircleFormat()
            {
                token = token,
                friendId = data,
                circleName = circleName
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://api.versine.fr/circles/removeFromCircle", httpContent);
            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            if ((string) json.status == "success")
            {
                return RedirectToPage("/ManageCircle", new { name = circleName });
            }
            else
            {
                return RedirectToPage("/ManageCircle", new { name = circleName });
            }
        }
        public async Task<ActionResult> OnPostAddToCircleAsync(string data)
        {
            string circleName = (string) StorageManager.storage.Get("currentCircleName");
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");
            AddToCircleFormat bodyObject = new AddToCircleFormat()
            {
                token = token,
                friendId = data,
                circleName = circleName
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://api.versine.fr/circles/addToCircle", httpContent);
            string bodyString = await result.Content.ReadAsStringAsync();

            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            if ((string) json.status == "success")
            {
                return RedirectToPage("/ManageCircle", new { name = circleName });
            }
            else
            {
                return RedirectToPage("/ManageCircle", new { name = circleName });
            }
        }

        public async Task<ActionResult> OnPostDeleteCircleAsync(string data)
        {
            string circleName = (string) StorageManager.storage.Get("currentCircleName");
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");
            DeleteCircleFormat bodyObject = new DeleteCircleFormat()
            {
                token = token,
                circleName = circleName,
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://api.versine.fr/circles/deleteCircle", httpContent);
            string bodyString = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            if ((string) json.status == "success")
            {
                return RedirectToPage("/Circles");
            }
            else
            {
                return RedirectToPage("/ManageCircle", new { name = circleName });
            }
        }

    }
}
