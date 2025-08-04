using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sakenny.Application.DTO
{
    public class UserHostRequestDTO
    {
        public string Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string ProfilePicUrl { get; set; }
        public string UrlIdFront { get; set; }
        public string UrlIdBack { get; set; }
    }
}