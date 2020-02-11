using System;
using System.Collections.Generic;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface IGroupResult
    {
        bool Success { get; }

        IList<INotificationResult> NotificationResults { get; set; }

        IMediator Mediator { get; set; }
    }
}
