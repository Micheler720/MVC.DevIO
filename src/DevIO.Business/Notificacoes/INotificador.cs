using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.App.Notificacoes
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacao();
        void Handle(Notificacao notificacao); 
    }
}
