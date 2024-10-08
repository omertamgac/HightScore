﻿using HighScore.Entities.Model.Concrete;

namespace HighScore.Models
{
    public class UserListViewModel
    {
        public IEnumerable<MetaUser> Users { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<string> SelectedRoles { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
    }
}
