﻿using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class BookSubject:IEntity
    {
        public Guid Id { get; set; }

        public string SubjectName { get; set; } = "";

        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
