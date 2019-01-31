using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_DEMO.ViewModel
{
   
    public class SendMailViewModel
    {
        public string Account { get; set; }

        public string Email { get; set; }

        public string dosomething { get; set; }
    }

    public class UploadImage
    {
        public string Account { get; set; }

        public string base64data { get; set; }
    }
}
