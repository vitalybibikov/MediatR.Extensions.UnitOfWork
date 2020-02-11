using System.Collections.Generic;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork.Results
{
    public class NotificationResult: INotificationResult
    {
        public bool Success { get; set; }
        public IEnumerable<INotification> FireOnFailure { get; set; }
        public IEnumerable<INotification> FireOnSuccess { get; set; }
    }
}
