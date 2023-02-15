using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agric.Models.ViewModel
{
    public class UserModel
    {
        [AllowHtml]
        public string Username { get; set; }
        [AllowHtml]
        public string Password { get; set; }
    }
}