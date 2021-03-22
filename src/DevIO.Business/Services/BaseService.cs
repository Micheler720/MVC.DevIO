using DevIO.App.Notificacoes;
using DevIO.Business.Models;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.App.Services
{
    public abstract class  BaseService
    {
        private readonly INotificador _notificador;

        public BaseService(INotificador notificador)
        {
            _notificador = notificador;
        }
        
        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string message)
        {
            _notificador.Handle(new Notificacao(message));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity 
        {
            var validator = validacao.Validate(entidade);
            if (validator.IsValid) return true;

            Notificar(validator);
            return false;
        }
    }
}
