using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.BaseModels
{
    public class CustomModelError
    {
        public int StatusCode { get; set; }
        public string CustomErrorMessage { get; set; } = "";
    }
}
