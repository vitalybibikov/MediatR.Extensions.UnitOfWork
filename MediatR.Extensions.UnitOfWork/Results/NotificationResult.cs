using System.Collections.Generic;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork.Results
{
    public class NotificationResult : INotificationResult
    {
        public bool Success { get; set; }

        public IList<INotification> OnFailedNotifications { get; set; } = new List<INotification>();

        public IList<INotification> OnSucceededNotifications { get; set; } = new List<INotification>();
    }
}
