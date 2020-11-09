using CineMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class UserListViewModel : PaginationBase
    {
        public List<UserRoleViewModel> Users { get; set; }
    }
}