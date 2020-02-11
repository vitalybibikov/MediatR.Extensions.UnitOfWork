using System.Collections.Generic;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork.Results
{
    public class NotificationResult : INotificationResult
    {
        public bool Success { get; set; }

        public IEnumerable<INotification> OnFailedNotifications { get; set; } = new List<INotification>();

        public IEnumerable<INotification> OnSucceedNotifications { get; set; } = new List<INotification>();
    }
}
