using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class UserRoleViewModel
    {
        [Display(Name ="User ID")]
        public string UserId { get; set; }
        [Display(Name ="Role name")]
        public string RoleName { get; set; }
        [Display(Name ="Username")]
        public string UserName { get; set; }
    }
}