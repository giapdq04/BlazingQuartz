﻿namespace BlazeQuartz.Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Role_Id { get; set; }
        public string Role { get; set; }
        public string GROUP_NAME { get; set; }
    }
}
