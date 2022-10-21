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
    public class PacienteRepository
    {
        public Paciente Save(Paciente paciente, MySqlConnection conn)
        {
            try
            {
                paciente.Pessoa.Id = SavePessoa(paciente, conn);
                MySqlCommand cmd = new MySqlCommand(SQL_INSERT_PACIENTE, conn);

                cmd.Parameters.Add(@"id_pessoa", MySqlDbType.Int32).Value = paciente.Pessoa.Id;
                cmd.Parameters.Add(@"id_convenio", MySqlDbType.Int32).Value = paciente.Convenio.Id;
                cmd.Parameters.Add(@"numero_prontuario", MySqlDbType.Int32).Value = paciente.NrProntuario;
                cmd.Parameters.Add(@"paciente_risco", MySqlDbType.VarChar, 10).Value = paciente.PacienteRisco;
                cmd.ExecuteNonQuery();

                return paciente;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Ocorreu um erro no MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        
        private Int32 SavePessoa(Paciente paciente, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_INSERT_PESSOA, conn);
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar, 55).Value = paciente.Pessoa.Nome;
                cmd.Parameters.Add("@cgccpf", MySqlDbType.VarChar, 18).Value = paciente.Pessoa.CGCCPF;
                cmd.Parameters.Add("@tipo_pessoa", MySqlDbType.Enum).Value = paciente.Pessoa.TipoPessoa;
                cmd.ExecuteNonQuery();
                return (Int32)cmd.LastInsertedId;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Ocorreu um erro no MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        //git commit 21/10
        public Paciente UpdatePessoa(Paciente paciente, Int32 idAlterar, MySqlConnection conn)
        {
            try
            {
                MySqlCommand cmdPessoa = new MySqlCommand(SQL_UPDATE_PESSOA, conn);
                cmdPessoa.Parameters.Add("@nome", MySqlDbType.VarChar, 55).Value = paciente.Pessoa.Nome;
                cmdPessoa.Parameters.Add("@fl_status", MySqlDbType.Enum).Value = paciente.Pessoa.Status;
                cmdPessoa.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = paciente.Pessoa.Id;
                MySqlCommand cmdPaciente = new MySqlCommand(SQL_UPDATE_PACIENTE, conn);
                cmdPaciente.Parameters.Add("@fl_status", MySqlDbType.Enum).Value = paciente.Status;
                cmdPaciente.Parameters.Add("@paciente_risco", MySqlDbType.VarChar, 10).Value = paciente.PacienteRisco;
                cmdPaciente.Parameters.Add("@fl_obito", MySqlDbType.Enum).Value = paciente.FlObito;
                cmdPaciente.Parameters.Add("@id_convenio", MySqlDbType.Int32).Value = paciente.Convenio.Id;
                cmdPaciente.Parameters.Add("@id", MySqlDbType.Int32).Value = idAlterar;
                cmdPessoa.ExecuteNonQuery();
                cmdPaciente.ExecuteNonQuery();

                return paciente;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Ocorreu um erro no MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void ExcluirPaciente(Int32 id_pessoa, Int32 id_paciente)
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();
            try
            {
                MySqlCommand cmdPessoa = new MySqlCommand(SQL_DELETE_PESSOA, conn);
                cmdPessoa.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = id_pessoa;
                MySqlCommand cmdPaciente = new MySqlCommand(SQL_DELETE_PACIENTE, conn);
                cmdPaciente.Parameters.Add("@id_paciente", MySqlDbType.Int32).Value = id_paciente;
                cmdPaciente.ExecuteNonQuery();
                cmdPessoa.ExecuteNonQuery();
                MessageBox.Show("Paciente Excluído com Sucesso!", "Excluir Paciente", MessageBoxButtons.OK);
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public MySqlDataReader GetPaciente(Int32 id_pessoa)
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_SELECT_PACIENTE, conn);
                cmd.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = id_pessoa;
                MySqlDataReader pacienteData = cmd.ExecuteReader();
                return pacienteData;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public MySqlDataReader GetPessoa(Int32 id_pessoa)
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();
            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_SELECT_PESSOA, conn);
                cmd.Parameters.Add("@id_pessoa", MySqlDbType.Int32).Value = id_pessoa;
                MySqlDataReader pessoaData = cmd.ExecuteReader();
                return pessoaData;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        internal MySqlDataReader GetAllPessoas()
        {
            MySqlConnection conn = ConnectionMySQL.GetConnection();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SQL_SELECT_PACIENTES, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                return dataReader;
            }
            catch (MySqlException myExc)
            {
                MessageBox.Show(myExc.Message, "Erro de MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        #region SQLs

        private const String SQL_SELECT_PACIENTE = @"select * from paciente where id_pessoa = @id_pessoa";
        private const String SQL_SELECT_PESSOA = @"SELECT * from pessoa where id = @id_pessoa";
        private const String SQL_SELECT_PACIENTES = @"select  
	pe.id as Id_Pessoa, 
	pe.fl_status as 'Status', 
	pe.nome as Nome, 
	pe.cgccpf as CPF, 
	pa.numero_prontuario as N_Prontuário, 
	pa.paciente_risco as Risco, 
    pa.fl_obito as 'Obito?',
    c.id as Id_Convenio,
	c.nome as Convenio,
    e.UF,
    e.Cidade,
    e.CEP,
    e.Bairro,
    e.Rua,
    e.Numero
from paciente pa
	inner join pessoa pe on pa.id_pessoa = pe.id
    inner join convenio c on pa.id_convenio = c.id
    inner join endereco e on pe.id = e.id_pessoa;";

        private const String SQL_INSERT_PESSOA = @"INSERT INTO pessoa
(nome,
cgccpf,
tipo_pessoa,
fl_status)
VALUES
(@nome,
@cgccpf,
@tipo_pessoa,
'A')";
        private const String SQL_INSERT_PACIENTE = @"INSERT INTO paciente
(id_pessoa,
id_convenio,
numero_prontuario,
paciente_risco,
fl_status,
fl_obito)
VALUES
(@id_pessoa,
@id_convenio,
@numero_prontuario,
@paciente_risco,
'A',
0)";

        private const String SQL_UPDATE_PESSOA = @"UPDATE pessoa
SET nome = @nome,
fl_status = @fl_status
where id = @id_pessoa;";
        private const String SQL_UPDATE_PACIENTE = @"UPDATE paciente
set fl_status = @fl_status,
paciente_risco = @paciente_risco,
fl_obito = @fl_obito,
id_convenio = @id_convenio
where id_pessoa = @id;";

        private const String SQL_DELETE_PESSOA = @"delete from pessoa where id = @id_pessoa";
        private const String SQL_DELETE_PACIENTE = @"delete from paciente where id = @id_paciente";

        #endregion
    }
}
