using Library.DataAccess;
using Library.Models.BaseModels;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Services
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private DataContext _dataContext;

        public NotificationService(DataContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}
