using DevIO.App.Interface;
using DevIO.App.Models.Validations;
using DevIO.App.Notificacoes;
using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.App.Services
{
    public class ProdutoService : BaseService, IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository,
                               INotificador notificador)
            : base(notificador)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task Adicionar(Produto produto)
        {
            var validator = new ProdutoValidation();
            if (!ExecutarValidacao(validator, produto)) return;

            await _produtoRepository.Adicionar(produto);
            return;
        }

        public async Task Atualizar(Produto produto)
        {
            var validator = new ProdutoValidation();
            if (!ExecutarValidacao(validator, produto)) return;
            await _produtoRepository.Atualizar(produto);
            return;
        }

        public async Task Remover(Guid id)
        {
            await _produtoRepository.Remover(id);
        }

        public void Dispose()
        {
            _produtoRepository?.Dispose();
        }
    }
}
