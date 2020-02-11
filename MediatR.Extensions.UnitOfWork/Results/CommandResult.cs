using System;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork.Results
{
    public class CommandResult : ICommandResult
    {
        public bool Success { get; set; }
    }
}
