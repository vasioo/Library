﻿using Library.DataAccess.MainModels;
using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class Notification : IEntity
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public ApplicationUser Sender { get; set; } = new ApplicationUser();
        public ApplicationUser Receiver { get; set; } = new ApplicationUser();
        public DateTime DateOfSending { get; set; } = DateTime.Now;
    }
}