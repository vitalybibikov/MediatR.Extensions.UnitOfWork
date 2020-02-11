using System;
using System.Threading.Tasks;
using System.Transactions;
using MediatR.Extensions.UnitOfWork.Interface;

namespace MediatR.Extensions.UnitOfWork
{
    public static class UnitOfWorkExtensions
    {
        public static async Task<ICommandResult> Chain<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse: ICommandResult
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            ICommandResult response = null;

            foreach (var request in requests)
            {
                response = await mediator.Send(request);
                if (!response.Success)
                {
                    return response;
                }
            }

            return response;
        }

        public static async Task<ICommandResult> ChainInTransaction<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse: ICommandResult
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result =await Chain(mediator, requests);

                if (result.Success)
                {
                    scope.Complete();
                }

                return result;
            }
        }

        public static async Task<ICommandResult> SendAndFire<TResponse>(
            this IMediator mediator,
            IRequest<TResponse> request) 
            where TResponse: INotificationResult
        {
            var response = await mediator.Send(request);

            if (response.Success && response.FireOnSuccess != null)
            {
                foreach (var notification in response.FireOnSuccess)
                {
                    await mediator.Publish(notification);
                }
            }
            else if (!response.Success && response.FireOnFailure != null)
            {
                foreach (var notification in response.FireOnFailure)
                {
                    await mediator.Publish(notification);
                }
            }

            return response;
        }

        public static async Task<ICommandResult> SendAndFireInTransaction<TResponse>(
            this IMediator mediator,
            IRequest<TResponse> request) 
            where TResponse: INotificationResult
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var response = await mediator.Send(request);

                if (response.Success && response.FireOnSuccess != null)
                {
                    foreach (var notification in response.FireOnSuccess)
                    {
                        await mediator.Publish(notification);
                    }
                }
                else if (!response.Success && response.FireOnFailure != null)
                {
                    foreach (var notification in response.FireOnFailure)
                    {
                        await mediator.Publish(notification);
                    }
                }

                if (response.Success)
                {
                    scope.Complete();
                }

                return response;
            }
        }
    }
}
