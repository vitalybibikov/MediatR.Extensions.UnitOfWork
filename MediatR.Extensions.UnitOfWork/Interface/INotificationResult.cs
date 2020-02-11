using System.Collections.Generic;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface INotificationResult : ICommandResult
    {
        IEnumerable<INotification> OnFailedNotifications { get; set; }

        IEnumerable<INotification> OnSucceedNotifications { get; set; }
    }
}
