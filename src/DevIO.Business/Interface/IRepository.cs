using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.App.Interface
{
    public interface IRepository<TEntity> : IDisposable where TEntity: Entity
    {
        Task Adicionar(TEntity obj);
        Task<TEntity> ObterPorId(Guid id);
        Task<List<TEntity>> ObterTodos();
        Task Atualizar(TEntity obj);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task Remover(Guid id);
        Task<int> SaveChanges();
    }
}
