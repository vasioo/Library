﻿using Library.Models.UserModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.BaseModels
{
    public class Book : IEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime DateOfBookCreation { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBookPublishment { get; set; } = DateTime.Now;
        public BookCategory? Genre { get; set; }
        public string Description { get; set; } = "";
        public Membership NeededMembership { get; set; } = new Membership();
        public string Language { get; set; } = "";
        public string ISBN { get; set; } = "";
        public int AmountOfBooks { get; set; }
        public string BookPreviewLink { get; set; } = "";
        public ICollection<FavouriteBooks> FavouriteBooks { get; set; }
    }
}
