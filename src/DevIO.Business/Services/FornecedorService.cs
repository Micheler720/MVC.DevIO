using DevIO.App.Interface;
using DevIO.App.Models.Validations;
using DevIO.App.Notificacoes;
using DevIO.Business.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.App.Services
{
    public class FornecedorService : BaseService, IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
       

        public FornecedorService(IFornecedorRepository fornecedorRepository,
                               IEnderecoRepository enderecoRepository,
                               INotificador notificador)
            :base(notificador)
        {
            _enderecoRepository = enderecoRepository;
            _fornecedorRepository = fornecedorRepository;
        }
        public async Task Adicionar(Fornecedor fornecedor)
        {
            var validatorFornecedor = new FornecedorValidation();
            var validatorEndereco = new EnderecoValidation();

            if (!ExecutarValidacao(validatorFornecedor, fornecedor) ||
                !ExecutarValidacao(validatorEndereco, fornecedor.Endereco) ) return;


            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento).Result.Any())
            {
                Notificar("Já existe um fornecedor com este documento informado.");
                return;
            }

            await _fornecedorRepository.Adicionar(fornecedor);
            return;
        }

        public async Task Atualizar(Fornecedor fornecedor)
        {
            var validator = new FornecedorValidation();
            if (!ExecutarValidacao(validator, fornecedor)) return;

            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento && f.Id != fornecedor.Id).Result.Any())
            {
                Notificar("Já existe um fornecedor com este documento informado.");
                return;
            }

            await _fornecedorRepository.Atualizar(fornecedor);
            return;
        }

        public async Task AtualizarEndereco(Endereco endereco)
        {
            var validator = new EnderecoValidation();
            if (!ExecutarValidacao(validator, endereco)) return;

            await _enderecoRepository.Atualizar(endereco);

            return;
        }


        public async Task Remover(Guid id)
        {
            if(_fornecedorRepository.ObterFornecedorProdutosEndereco(id).Result.Produtos.Any())
            {
                Notificar("O fornecedor possui produtos cadastrados! ");
                return;
            }

            await _fornecedorRepository.Remover(id);
            return;
        }
        public void Dispose()
        {
            _enderecoRepository?.Dispose();
            _fornecedorRepository?.Dispose();
        }
    }
}
