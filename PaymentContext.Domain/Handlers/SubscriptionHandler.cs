using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : 
        Notifiable,
        IHandler<CreateBoletoSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        
        public SubscriptionHandler(IStudentRepository repository )
        {
            _repository = repository;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //verificar se documeno já esta cadastrado
            //verificar se emil ja esta cadastrado
            //gerar os vos
            //gerar as entidades
            //aplicar as validadções
            //salvar as informações
            //enviar email de boas vindas
            //retornar informaões

            return new CommandResult(true, "Signature successfully completed");
        }
    }
}