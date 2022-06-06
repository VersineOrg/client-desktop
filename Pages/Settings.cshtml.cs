using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace client_desktop.Pages
{
    
    public class Settings : PageModel
    {

        public ActionResult OnPostLogout(string data)
        {
            StorageManager.storage.Clear();
            return RedirectToPage("/Index");
        }
        
        public async Task OnGet()
        {
             
             
        }

    }
}
