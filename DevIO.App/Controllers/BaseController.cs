using AutoMapper;
using DevIO.App.Notificacoes;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IMapper _mapper;
        private readonly INotificador _notificador;
        public BaseController(IMapper mapper, INotificador notificador)
        {
            _mapper = mapper;
            _notificador = notificador;
        }

        protected bool OperacaoValida ()
        {
            return !_notificador.TemNotificacao();
        }


    }
}
