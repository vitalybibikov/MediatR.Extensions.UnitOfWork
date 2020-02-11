using System;
using System.Collections.Generic;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface IGroupResult
    {
        bool Success { get; }

        List<INotificationResult> NotificationResults { get; set; }

        IMediator Mediator { get; set; }
    }
}
