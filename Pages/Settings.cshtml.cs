using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class DeleteUserFormat
     {
         public string token { get; set; }
     }
    public class Settings : PageModel
    {

        public ActionResult OnPostLogout(string data)
        {
            StorageManager.storage.Clear();
            return RedirectToPage("/Index");
        }
        public async Task<ActionResult> OnPostDelete(string data)
        {
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");
            PostFormat bodyObject = new PostFormat()
            {
                token = token,
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            string baseurl = "http://localhost:7000/user/delete";
            var result = await client.PostAsync(baseurl, httpContent);
            string? bodyString = await result.Content.ReadAsStringAsync();
            Console.WriteLine(bodyString);
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            Console.WriteLine(json);
            
            StorageManager.storage.Clear();
            return RedirectToPage("/Index");
        }
        public async Task OnGet()
        {
             
             
        }

    }
}