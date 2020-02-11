using System;
using System.Collections.Generic;
using System.Text;

namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface IBuilder
    {
        IMediator Mediator { get; set; }

        IEnumerable<IBuilderResult> Builders { get; set; }

        IRequest<ICommandResult>[] Requests { get; set; }
    }
}
