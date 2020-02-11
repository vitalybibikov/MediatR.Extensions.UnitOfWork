# MediatR.Extensions.UnitOfWork

Set of extensions to run multiple MediatR commands sequentially. 
- Simple sequence of commands
- Sequence in a transaction scope,  
- When you need to fire a notification just after a command succeded/failed without any changes in a handler.

Mostly solves the problem of nested commands, reduces command size, each command stays atomic and might be tested simply as it governs only 1 responsility at a time.


            var command1 = new CreatePurchaseCommand
            {
                WorkerId = userId,
                UserId = model.UserId,
                ProductId = model.ProductId,
                VenueId = venueId,
                Value = model.Purchase,
                LoyaltyProductGroupId = model.LoyaltyProductGroupId
            };

            var command2 = new BurnPurchaseCommand
            {
                WorkerId = userId,
                UserId = model.UserId,
                VenueId = venueId,
                Amount = model.Burn,
                LoyaltyProductGroupId = model.LoyaltyProductGroupId
            };
            
            var result1 = await Mediator.Chain(command1, command2); //OR in transaction to commit or rollback them both
            var result2 = await Mediator.ChainInTransaction(command1, command2);
