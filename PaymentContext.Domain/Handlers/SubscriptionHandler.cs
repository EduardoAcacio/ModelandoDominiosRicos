using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;
using PaymentContext.Domain.Repositories;
using Flunt.Validations;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Entities;
using System;
using PaymentContext.Domain.Services;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : 
        Notifiable,
        IHandler<CreateBoletoSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;
        
        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //Fail Fast Validations
            command.Validate();
            if(command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false,"Your subscription was not possible.");
            }

            //verificar se documento já esta cadastrado
            if(_repository.DocumentExists(command.Document))
            {
                AddNotification("Documento","This CPF is already registered");
            }

            //verificar se emil ja esta cadastrado
            if(_repository.EmailExists(command.Document))
            {
                AddNotification("Email","This Email is already registered");
            }

            //gerar os vos
            var name = new Name(command.FirstName,command.LastName);
            var document = new Document(command.Document,EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street,command.Number,command.Neighborhood,command.City,command.State,command.Country,command.ZipCode); 

            //gerar as entidades
            var student = new Student(name,document,email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode,
                command.BoletoNumber,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PaymentNumber,command.PayerDocumentType),
                address,
                email);
            
            //Relacionamento
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Agrupar as validadções
            AddNotifications(name,document,email,student,subscription, payment);

            //Salvar as informações
            _repository.CreateSubscription(student);

            //enviar email de boas vindas
            _emailService.Send(student.Name.ToString(),student.Email.Address,"Welcome PaymentContext","Signature successfully completed");

            //retornar informaões
            return new CommandResult(true, "Signature successfully completed");
        }
    }
}