# MediatR.Extensions.UnitOfWork

Set of extensions to run multiple MediatR commands sequentially. 
* Simple sequence of commands
* Sequence in a transaction scope,  



            var command1 = new CreatePurchaseCommand : ICommandResult
            {
                WorkerId = userId,
            };

            var command2 = new BurnPurchaseCommand : ICommandResult
            {
                WorkerId = userId,
            };

            var result1 = await Mediator.Chain(command1, command2); 
            var result2 = await Mediator.ChainScoped(command1, command2); //OR in transaction to commit or rollback them both


When you need to fire a notification just after a command succeded/failed without any changes in a handler.
  Creates a proper contract, firing of a notification after the command becomes more transparent.


            var command3 = new BurnPurchaseAndNotifyCommand : INotificationResult
            {
                WorkerId = userId,
            };
            
            var result1 = await Mediator.SendThenPublish(command1, command2); 

This allows to execute multiple commands with notifications, which should fail or succeed simultaniously. 
* Commands are not nested
* And can be executed in a scope
* Notifications are executed afterwards, means it becomes transparent
* You can update your app, without changes in commands that are already tested.
* Each command becomes atomic, and knows only about itself and notifications.
