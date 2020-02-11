namespace MediatR.Extensions.UnitOfWork.Interface
{
    public interface ICommandResult
    {
        bool Success { get; set; }
    }
}
