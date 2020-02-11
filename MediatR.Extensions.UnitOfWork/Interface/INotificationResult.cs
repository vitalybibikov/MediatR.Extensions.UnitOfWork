using System.Collections.Generic;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface INotificationResult : ICommandResult
    {
        IList<INotification> OnFailedNotifications { get; set; }

        IList<INotification> OnSucceededNotifications { get; set; }
    }
}
