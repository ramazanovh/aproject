﻿namespace AProjectMVC.Areas.admin.ViewModels.Account
{
    public class UserVM
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public IList<string> RoleName { get; set; }
    }
}
