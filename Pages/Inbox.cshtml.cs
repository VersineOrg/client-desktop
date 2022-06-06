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
    
    public class InboxModel : PageModel
    {

        [BindProperty]
        public string? Username { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("/Profile", new { username = Username });
        }

        public void OnGet()
        {
            
        }
        
    }
}
