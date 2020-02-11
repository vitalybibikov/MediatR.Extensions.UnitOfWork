using System.Collections.Generic;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface INotificationResult : ICommandResult
    {
        IEnumerable<INotification> FireOnFailure { get; set; }

        IEnumerable<INotification> FireOnSuccess { get; set; }
    }
}
