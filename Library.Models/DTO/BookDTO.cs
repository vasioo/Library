﻿using Library.Models.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.DTO
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime DateOfBookCreation { get; set; }
        public string? Genre { get; set; }
        public string Description { get; set; } = "";
        public int AvailableItems { get; set; }
        public string NeededMembership { get; set; } = "";
    }
}
