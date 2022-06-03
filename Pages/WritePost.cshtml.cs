using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    public class PostFormat
     {
         public string token { get; set; }
         public string message { get; set;}
     }
    public class WritePost : PageModel
    {
        [BindProperty]
        public string? Content { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
            using var client = new HttpClient();
            string token = (string) StorageManager.storage.Get("token");
            Console.WriteLine("CONTENT: " + Content);
            PostFormat bodyObject = new PostFormat()
            {
                token = token,
                message = Content
            };
            string requestBody = JsonConvert.SerializeObject(bodyObject);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            string baseurl = "http://api.versine.fr/posts/addPost";
            var result = await client.PostAsync(baseurl, httpContent);
            string? bodyString = await result.Content.ReadAsStringAsync();
            Console.WriteLine(bodyString);
            dynamic json = JsonConvert.DeserializeObject(bodyString)!;
            Console.WriteLine(json);
            
            
            return RedirectToPage("/App", new { msg = "Success" });

        }
        public async Task OnGet()
        {
            
        }
    }
}