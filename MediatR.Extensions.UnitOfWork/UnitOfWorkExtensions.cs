using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MediatR.Extensions.UnitOfWork.Interface;
using MediatR.Extensions.UnitOfWork.Results;

namespace MediatR.Extensions.UnitOfWork
{
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Runs several commands one after another
        /// </summary>
        public static async Task<ICommandResult> Chain<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse : ICommandResult
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

        /// <summary>
        /// Runs several commands one after another in a TransactionScope.
        /// </summary>
        public static async Task<ICommandResult> ChainScoped<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse : ICommandResult
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await Chain(mediator, requests);

            if (result.Success)
            {
                scope.Complete();
            }

            return result;
        }

        /// <summary>
        /// Runs a command and executes it's notifications after it.
        /// </summary>
        public static async Task<ICommandResult> SendThenPublish<TResponse>(
            this IMediator mediator,
            IRequest<TResponse> request) 
            where TResponse : INotificationResult
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await mediator.Send(request);

            if (response.Success && response.OnSucceededNotifications != null)
            {
                foreach (var notification in response.OnSucceededNotifications)
                {
                    await mediator.Publish(notification);
                }
            }
            else if (!response.Success && response.OnFailedNotifications != null)
            {
                foreach (var notification in response.OnFailedNotifications)
                {
                    await mediator.Publish(notification);
                }
            }

            return response;
        }

        /// <summary>
        /// Runs several commands in a transaction scope, then publishes all failed/succeed notifications.
        /// </summary>
        public static async Task<ICommandResult> RunAllScopedThenPublish<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse : INotificationResult
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var responses = new List<INotificationResult>();

            foreach (var request in requests)
            {
                var response = await mediator.Send(request);
                responses.Add(response);
            }

            var result = responses.Select(x => x.Success)
                .Aggregate((x, y) => x && y);

            if (result)
            {
                scope.Complete();
            }

            foreach (var response in responses)
            {
                foreach (var notification in response.OnSucceededNotifications)
                {
                    await mediator.Publish(notification);
                }

                foreach (var notification in response.OnFailedNotifications)
                {
                    await mediator.Publish(notification);
                }
            }

            return new CommandResult
            {
                Success = result
            };
        }

        /// <summary>
        /// Runs several commands in a transaction scope, returns groupd notification responses.
        /// </summary>
        public static async Task<IGroupResult> RunAllScoped<TResponse>(
            this IMediator mediator,
            params IRequest<TResponse>[] requests) 
            where TResponse : INotificationResult
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var responses = new List<INotificationResult>();

            foreach (var request in requests)
            {
                var response = await mediator.Send(request);
                responses.Add(response);
            }

            var result = responses.Select(x => x.Success)
                .Aggregate((x, y) => x && y);

            if (result)
            {
                scope.Complete();
            }

            return new GroupResult()
            {
                NotificationResults = responses,
                Mediator = mediator
            };
        }

        /// <summary>
        /// Publishes grouped notifications both succeed and failed.
        /// </summary>
        public static async Task ThenPublishAll(
            this Task<IGroupResult> awaitedResult)
        {
            if (awaitedResult == null)
            {
                throw new ArgumentNullException(nameof(awaitedResult));
            }

            var groupResult = await awaitedResult;

            if (groupResult.Mediator == null)
            {
                throw new ArgumentException(nameof(groupResult.Mediator));
            }

            foreach (var response in groupResult.NotificationResults)
            {
                foreach (var notification in response.OnSucceededNotifications)
                {
                    await groupResult.Mediator.Publish(notification);
                }

                foreach (var notification in response.OnFailedNotifications)
                {
                    await groupResult.Mediator.Publish(notification);
                }
            }
        }

        /// <summary>
        /// Publishes grouped succeed notifications.
        /// </summary>
        public static async Task ThenPublishSucceeded(
            this Task<IGroupResult> awaitedResult)
        {
            if (awaitedResult == null)
            {
                throw new ArgumentNullException(nameof(awaitedResult));
            }

            var groupResult = await awaitedResult;

            if (groupResult.Mediator == null)
            {
                throw new ArgumentException(nameof(groupResult.Mediator));
            }

            foreach (var notification in groupResult.NotificationResults
                .SelectMany(response => response.OnSucceededNotifications))
            {
                await groupResult.Mediator.Publish(notification);
            }
        }

        /// <summary>
        /// Publishes grouped failed notifications.
        /// </summary>
        public static async Task ThenPublishFailed(
            this Task<IGroupResult> awaitedResult)
        {
            if (awaitedResult == null)
            {
                throw new ArgumentNullException(nameof(awaitedResult));
            }

            var groupResult = await awaitedResult;

            if (groupResult.Mediator == null)
            {
                throw new ArgumentException(nameof(groupResult.Mediator));
            }

            foreach (var notification in groupResult.NotificationResults
                .SelectMany(response => response.OnFailedNotifications))
            {
                await groupResult.Mediator.Publish(notification);
            }
        }
    }
}
