using Library.Models.UserModels.Interfaces;

namespace Library.Models.BaseModels
{
    public class Membership:IEntity
    {
        public Guid Id { get; set; }
        public string MembershipName { get; set; } = "";
        public int EndAmountOfPoints{ get; set; }
        public int StartingNeededAmountOfPoints{ get; set; }
    }
}
