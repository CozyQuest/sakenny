using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sakenny.Application.DTO
{
    public class BecomeHostRequest
    {
        [FromForm(Name = "BackImage")]
        public IFormFile BackImage { get; set; }
        [FromForm(Name = "FrontImage")]
        public IFormFile FrontImage { get; set; }
    }
}