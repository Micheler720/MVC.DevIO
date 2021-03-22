using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.App.Interface
{
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);
    }
}
