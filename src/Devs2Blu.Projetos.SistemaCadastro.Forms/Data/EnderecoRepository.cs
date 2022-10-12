using Devs2Blu.Projetos.SistemaCadastro.Models;
using Devs2Blu.Projetos.SistemaCadastro.Models.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Devs2Blu.Projetos.SistemaCadastro.Forms.Data
{
    public class EnderecoRepository
    {
        public Endereco SaveEndereco(Endereco endereco, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(SQL_INSERT_ENDERECO, conn);
            cmd.Parameters.Add("@id_Pessoa", MySqlDbType.Int32).Value = endereco.Pessoa.Id;
            cmd.Parameters.Add("@CEP", MySqlDbType.VarChar, 15).Value = endereco.CEP;
            cmd.Parameters.Add("@rua", MySqlDbType.VarChar, 50).Value = endereco.Rua;
            cmd.Parameters.Add("@numero", MySqlDbType.Int32).Value = endereco.Numero;
            cmd.Parameters.Add("@bairro", MySqlDbType.VarChar, 45).Value = endereco.Bairro;
            cmd.Parameters.Add("@cidade", MySqlDbType.VarChar, 30).Value = endereco.Cidade;
            cmd.Parameters.Add("@uf", MySqlDbType.VarChar, 2).Value = endereco.UF.Substring(0,1);
            cmd.ExecuteNonQuery();

            return endereco;
        }

        public Endereco UpdateEndereco(Endereco endereco, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(SQL_UPDATE_ENDERECO, conn);
            cmd.Parameters.Add("@cep", MySqlDbType.VarChar, 15).Value = endereco.CEP;
            cmd.Parameters.Add("@rua", MySqlDbType.VarChar, 50).Value = endereco.Rua;
            cmd.Parameters.Add("@numero", MySqlDbType.Int32).Value = endereco.Numero;
            cmd.Parameters.Add("@bairro", MySqlDbType.VarChar, 45).Value = endereco.Bairro;
            cmd.Parameters.Add("@cidade", MySqlDbType.VarChar, 30).Value = endereco.Cidade;
            cmd.Parameters.Add("@uf", MySqlDbType.VarChar, 2).Value = endereco.UF;
            cmd.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = endereco.Pessoa.Id;
            cmd.ExecuteNonQuery();

            return endereco;
        }

        public void ExcluirEndereco(Int32 id_pessoa) 
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_DELETE_ENDERECO, conn);
                cmd.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = id_pessoa;
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public MySqlDataReader GetEndereco(Int32 id_pessoa)
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_SELECT_ENDERECO, conn);
                cmd.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = id_pessoa;
                MySqlDataReader enderecoData = cmd.ExecuteReader();
                return enderecoData;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        #region SQLs

        private const String SQL_INSERT_ENDERECO = @"INSERT INTO endereco
(id_pessoa,
CEP,
rua,
numero,
bairro,
cidade,
uf)
VALUES
(@id_Pessoa,
@CEP,
@rua,
@numero,
@bairro,
@cidade,
@uf)";
        private const String SQL_SELECT_ENDERECO = @"select * from endereco where id_pessoa = @id_pessoa;";
        private const String SQL_UPDATE_ENDERECO = @"UPDATE endereco
set CEP = @cep,
rua = @rua,
numero = @numero,
bairro = @bairro,
cidade = @cidade,
uf = @uf
where id_pessoa = @id_pessoa;";
        private const String SQL_DELETE_ENDERECO = @"delete from endereco where id_pessoa = @id_pessoa";

        #endregion
    }
}
