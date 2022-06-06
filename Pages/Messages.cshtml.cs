using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace client_desktop.Pages
{
    
    public class MessagesModel : PageModel
    {
        public void OnGet()
        {
            List<string> friends = (List<string>) StorageManager.storage.Get("friends");

            foreach (string friend in friends)
            {
                Console.WriteLine(friend);
            }
            
        }
        
    }
}
