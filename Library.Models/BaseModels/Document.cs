using Library.Models.UserModels.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models.BaseModels
{
    public class Document:IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
    }
}
