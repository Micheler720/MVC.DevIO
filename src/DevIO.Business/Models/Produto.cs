using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.Business.Models
{
    public class Produto : Entity
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public Decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public Guid FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

    }
}
