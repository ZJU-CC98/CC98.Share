using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CC98.Share.ViewModels.FileController
{
    public class UploadViewModel
    {
        public IFormFile[] Files { get; set; }
        public int Value { get; set; }
    }
}
