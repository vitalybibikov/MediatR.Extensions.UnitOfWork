using System.Collections.Generic;
using System.Linq;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork.Results
{
    public class GroupResult : IGroupResult
    {
        public bool Success
        {
            get
            {
                return NotificationResults.Select(x => x.Success)
                    .Aggregate((x, y) => x && y);
            }
        }

        public List<INotificationResult> NotificationResults { get; set; } = new List<INotificationResult>();

        public IMediator Mediator { get; set; }
    }
}
