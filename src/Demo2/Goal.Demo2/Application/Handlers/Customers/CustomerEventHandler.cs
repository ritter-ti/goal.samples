using Goal.Demo2.Domain.Customers.Events;
using Goal.Demo2.Infra.Data.Query.Repositories.Customers;
using Goal.Demo2.Model.Customers;
using Goal.Seedwork.Domain.Events;

namespace Goal.Demo2.Application.Handlers.Customers
{
    public class CustomerEventHandler :
        IEventHandler<CustomerRegisteredEvent>,
        IEventHandler<CustomerUpdatedEvent>,
        IEventHandler<CustomerRemovedEvent>
    {
        private readonly ICustomerQueryRepository customerRepository;

        public CustomerEventHandler(ICustomerQueryRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public async Task Handle(CustomerUpdatedEvent message, CancellationToken cancellationToken)
        {
            CustomerModel customer = await customerRepository.LoadAsync(message.AggregateId, cancellationToken);

            customer.Name = message.Name;
            customer.BirthDate = message.BirthDate;
            customer.Email = message.Email;

            await customerRepository.StoreAsync(
                message.AggregateId,
                customer,
                cancellationToken);
        }

        public async Task Handle(CustomerRegisteredEvent message, CancellationToken cancellationToken)
        {
            await customerRepository.StoreAsync(
                message.AggregateId,
                new CustomerModel
                {
                    CustomerId = message.AggregateId,
                    Name = message.Name,
                    BirthDate = message.BirthDate,
                    Email = message.Email,
                },
                cancellationToken);
        }

        public async Task Handle(CustomerRemovedEvent message, CancellationToken cancellationToken)
            => await customerRepository.RemoveAsync(message.AggregateId, cancellationToken);
    }
}
