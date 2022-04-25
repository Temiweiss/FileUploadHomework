using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUploadHomework.data;

namespace FileUploadHomework.web.Models
{
    public class FileUploadViewModel
    {
         public Image Image { get; set; }
        public bool IsAllowed { get; set; }
    }
}
