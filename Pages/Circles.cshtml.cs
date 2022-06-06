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
    public class Circles : PageModel
    {
        public class CreateCircleFormat
        {
            public string? token { get; set; }
            public string? circleName { get; set; }
        }

        public async Task OnGet()
        {
            using var client = new HttpClient();

            string username = (string) StorageManager.storage.Get("username");
            string baseurl = "https://api.versine.fr/circles/userCircles";

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
                string jsoncircles = json.data;
                dynamic circles = JsonConvert.DeserializeObject(jsoncircles)!;

                StorageManager.storage.Store("circles", circles);
            }
            else {
                RedirectToPage("/App");
            }
             
        }


        [BindProperty]
        public string? CircleName { get; set; }

        public async Task<ActionResult> OnPostAsync(string data)
        {
            using var client = new HttpClient();

            string baseurl = "https://api.versine.fr/circles/createCircle";

            string token = StorageManager.storage.Get("token").ToString();

            if (CircleName != null)
            {
                CreateCircleFormat bodyObject = new CreateCircleFormat()
                {
                    token = token,
                    circleName = CircleName
                };

                string requestBody = JsonConvert.SerializeObject(bodyObject);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(baseurl, httpContent);

                string bodyString = await result.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(bodyString)!;

                if (json != null && (string) json.status == "success") {
                    return RedirectToPage("/Circles");
                }
                else {
                    return RedirectToPage("/Circles");
                }
            }
            else {
                return RedirectToPage("/Circles");
            }

        }
    }
}