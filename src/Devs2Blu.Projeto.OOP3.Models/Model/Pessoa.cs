using Devs2Blu.Projetos.SistemaCadastro.Models.Enum;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devs2Blu.Projetos.SistemaCadastro.Models
{
    public class Pessoa
    {
        public Int32 Id { get; set; }
        public String Nome { get; set; }
        public String CGCCPF { get; set; }
        public TipoPessoa TipoPessoa { get; set; }
        public FlStatus Status { get; set; }

        public Pessoa()
        {
            Status = FlStatus.A;
        }

        public Pessoa(int id, string nome, string cgccpf, TipoPessoa tipoPessoa, FlStatus status)
        {
            Id = id;
            Nome = nome;
            CGCCPF = cgccpf;
            TipoPessoa = tipoPessoa;
            Status = status;
        }
    }
}
