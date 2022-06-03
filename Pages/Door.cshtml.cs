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
    
    public class DoorModel : PageModel
    {
        private readonly ILogger<DoorModel> _logger;

        public DoorModel(ILogger<DoorModel> logger)
        {
            _logger = logger;
        }

        
        public void OnGet()
        {
            
        }
        
    }
}
