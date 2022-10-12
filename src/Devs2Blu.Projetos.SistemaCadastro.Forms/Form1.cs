using Devs2Blu.Projetos.SistemaCadastro.Forms.Data;
using Devs2Blu.Projetos.SistemaCadastro.Models.Enum;
using Devs2Blu.Projetos.SistemaCadastro.Models.Model;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Ubiety.Dns.Core;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Devs2Blu.Projetos.SistemaCadastro.Forms
{
    public partial class FormMain : Form
    {
        public MySqlConnection Conn { get; set; }
        public ConvenioRepository ConvenioRepository = new ConvenioRepository();
        public PacienteRepository PacienteRepository = new PacienteRepository();
        public EnderecoRepository EnderecoRepository = new EnderecoRepository();

        public FormMain()
        {
            InitializeComponent();
        }

        #region Methods

        private void PopulaComboConvenio()
        {
            var listConvenios = ConvenioRepository.FetchAll();

            cbox_Convenio.DataSource = new BindingSource(listConvenios, null);
            cbox_Convenio.DisplayMember = "nome";
            cbox_Convenio.ValueMember = "id";
        }

        private void PopulaComboStatus()
        {
            cbox_StatusPessoa.DataSource = Enum.GetValues(typeof(FlStatus));
        }

        private void PopulaDataGridPessoa()
        {
            var listPessoas = PacienteRepository.GetAllPessoas();
            grid_Pacientes.DataSource = new BindingSource(listPessoas, null);
        }

        private bool ValidaFormCadastro()
        {
            if (txtb_Nome.Text.Equals("")) { return false; }
            if (masktxtb_CGCCPF.Text.Equals("")) { return false; }
            if (cbox_Convenio.SelectedIndex.Equals(-1)) { return false; }
            if (txtb_Risco.Text.Equals("")) { return false; }
            if (masktxtb_CEP.Text.Equals("")) { return false; }
            if (cbox_UF.Text.Equals("")) { return false; }
            if (txtb_Cidade.Text.Equals("")) { return false; }
            if (txtb_Bairro.Text.Equals("")) { return false; }
            if (txtb_Rua.Text.Equals("")) { return false; }
            if (txtb_Numero.Text.Equals("")) { return false; }
            return true;
        }

        private Int32 geraProntuario()
        {
            Random rd = new Random();
            Int32 nrProntuario = Int32.Parse($"{rd.Next(10000, 99999)}{DateTime.Now.Second + 10}");
            return nrProntuario;
        }

        private void CadastraAlteraPaciente()
        {
            if (ValidaFormCadastro())
            {
                Paciente paciente = new Paciente();
                Endereco endereco = new Endereco();
                
                paciente.Pessoa.Nome = txtb_Nome.Text;
                paciente.Pessoa.CGCCPF = masktxtb_CGCCPF.Text;
                paciente.Convenio.Id = (int)cbox_Convenio.SelectedValue;
                paciente.PacienteRisco = txtb_Risco.Text;
                paciente.NrProntuario = geraProntuario();
                paciente.TipoPessoa = rbFisica.Checked ? TipoPessoa.PF : TipoPessoa.PF;

                endereco.CEP = masktxtb_CEP.Text;
                endereco.UF = cbox_UF.Text;
                endereco.Cidade = txtb_Cidade.Text;
                endereco.Bairro = txtb_Bairro.Text;
                endereco.Rua = txtb_Rua.Text;
                endereco.Numero = Int32.Parse(txtb_Numero.Text);

                MySqlConnection conn = ConnectionMySQL.GetConnection();
                if (rb_Cadastrar.Checked)
                {
                    var pacienteResult = PacienteRepository.Save(paciente, conn);
                    endereco.Pessoa = paciente.Pessoa;
                    var enderecoResult = EnderecoRepository.SaveEndereco(endereco, conn);

                    if (pacienteResult.Pessoa.Id > 0)
                    {
                        MessageBox.Show($"Pessoa {paciente.Pessoa.Id} - {paciente.Pessoa.Nome} salvo com sucesso!", "Adicionar Pessoa", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        PopulaDataGridPessoa();
                    }
                }
                else if (rb_Alterar.Checked)
                {
                    paciente.Pessoa.Id = Int32.Parse(txtb_idAlteracao.Text);
                    paciente.Status = (FlStatus)cbox_StatusPessoa.SelectedItem;
                    Int32 idBuscar = Int32.Parse(txtb_idAlteracao.Text);
                    var pacienteResult = PacienteRepository.UpdatePessoa(paciente, Int32.Parse(txtb_idAlteracao.Text), conn);
                    endereco.Pessoa = paciente.Pessoa;
                    var enderecoResult = EnderecoRepository.UpdateEndereco(endereco, conn);

                    MessageBox.Show($"Pessoa {paciente.Pessoa.Id} - {paciente.Pessoa.Nome} salvo com sucesso!", "Alteração de Cadastro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    PopulaDataGridPessoa();
                }


            }
        }

        public void searchCEP()
        {
            string cepSearch = masktxtb_CEP.Text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://viacep.com.br/ws/" + cepSearch + "/json/");
            request.AllowAutoRedirect = false;
            HttpWebResponse CheckServer = (HttpWebResponse)request.GetResponse();

            if (CheckServer.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Servidor indisponível");
                return; // Sai da rotina
            }

            using (Stream webStream = CheckServer.GetResponseStream())
            {
                if (webStream != null)
                {
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string response = responseReader.ReadToEnd();
                        response = Regex.Replace(response, "[{},]", string.Empty);
                        response = response.Replace("\"", "");

                        String[] substrings = response.Split('\n');

                        int cont = 0;
                        foreach (var substring in substrings)
                        {
                            if (cont == 1)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                if (valor[0] == "  erro")
                                {
                                    MessageBox.Show("CEP não encontrado");
                                    //Focus();
                                    return;
                                }
                            }

                            //Logradouro
                            if (cont == 2)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                //txtLogradouro.Text = valor[1];
                                txtb_Rua.Text = valor[1];
                            }

                            //Complemento
                            if (cont == 3)
                            {
                                string[] valor = substring.Split(":".ToCharArray());

                            }

                            //Bairro
                            if (cont == 4)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                txtb_Bairro.Text = valor[1];
                            }

                            //Localidade (Cidade)
                            if (cont == 5)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                txtb_Cidade.Text = valor[1];
                            }

                            //Estado (UF)
                            if (cont == 6)
                            {
                                string[] valor = substring.Split(":".ToCharArray());
                                cbox_UF.Text = valor[1].Replace("\"", "");
                            }

                            cont++;
                        }
                    }
                }
            }

        }
        #endregion

        #region Eventos

        private void Form1_Load(object sender, EventArgs e)
        {
            #region TesteConexao
            //Conn = ConnectionMySQL.GetConnection();

            //if (Conn.State == ConnectionState.Open)
            //{
            //    MessageBox.Show("Conexão efetuada com sucesso!", "Conexão ao MySQL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    Conn.Close();
            //}
            #endregion
            PopulaComboConvenio();
            PopulaDataGridPessoa();
            PopulaComboStatus();
        }

        private void rbFisica_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFisica.Checked)
            {
                lblCGCCPF.Text = "CPF";
                lblCGCCPF.Visible = true;
            }
        }

        private void rbJuridica_CheckedChanged(object sender, EventArgs e)
        {
            if (rbJuridica.Checked)
            {
                lblCGCCPF.Text = "CNPJ";
                lblCGCCPF.Visible = true;
            }
        }

        private void ibtnCadastrar_Click(object sender, EventArgs e)
        {
            CadastraAlteraPaciente();
        }

        private void ibtnInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cadastro de Paciente");
        }

        private void ibtnLimpar_Click(object sender, EventArgs e)
        {
            txtb_Nome.Clear();
            masktxtb_CGCCPF.Clear();
            cbox_StatusPessoa.Text = "";
            masktxtb_CEP.Clear();
            cbox_UF.Text = "";
            txtb_Cidade.Clear();
            txtb_Bairro.Clear();
            txtb_Rua.Clear();
            txtb_Numero.Clear();
            cbox_StatusPaciente.Text = "";
            txtb_Risco.Clear();
            cbox_Convenio.Text = "";
            cbox_Obito.Text = "";
            txtb_idExcluir.Clear();
            txtb_idAlteracao.Clear();
            rb_Cadastrar.Checked = true;
        }

        private void ibtnListar_Click(object sender, EventArgs e)
        {
            grid_Pacientes.Visible = true;
        }

        private void btn_Exluir_Click(object sender, EventArgs e)
        {
            Int32 idBuscar = Int32.Parse(txtb_idExcluir.Text);
            var pacienteData = PacienteRepository.GetPaciente(idBuscar);
            pacienteData.Read();
            EnderecoRepository.ExcluirEndereco(Int32.Parse(pacienteData.GetString("id_pessoa")));
            PacienteRepository.ExcluirPaciente(Int32.Parse(pacienteData.GetString("id_pessoa")), Int32.Parse(pacienteData.GetString("id")));
            PopulaDataGridPessoa();
        }

        private void rb_Cadastrar_CheckedChanged(object sender, EventArgs e)
        {
            txtb_idAlteracao.Visible = false;
            txtb_idAlteracao.Enabled = false;
            btn_BuscaPaciente.Visible = false;
            cbox_StatusPessoa.Enabled = false;
            cbox_StatusPaciente.Enabled = false;
            cbox_Obito.Enabled = false;
            masktxtb_CGCCPF.Enabled = true;
            rbFisica.Enabled = true;
            rbJuridica.Enabled = true;
            lbl_IdAlterar.Visible = false;
        }

        private void rb_Alterar_CheckedChanged(object sender, EventArgs e)
        {
            txtb_idAlteracao.Visible = true;
            txtb_idAlteracao.Enabled = true;
            btn_BuscaPaciente.Visible = true;
            cbox_StatusPessoa.Enabled = true;
            cbox_StatusPaciente.Enabled = true;
            cbox_Obito.Enabled = true;
            masktxtb_CGCCPF.Enabled = false;
            rbFisica.Enabled = false;
            rbJuridica.Enabled = false;
            lbl_IdAlterar.Visible = true;
        }

        private void btn_BuscaPaciente_Click(object sender, EventArgs e)
        {
            Int32 idBuscar = Int32.Parse(txtb_idAlteracao.Text);
            var pacienteData = PacienteRepository.GetPaciente(idBuscar);
            pacienteData.Read();
            var pessoaData = PacienteRepository.GetPessoa(Int32.Parse(pacienteData.GetString("id_pessoa")));
            pessoaData.Read();
            var enderecoData = EnderecoRepository.GetEndereco(Int32.Parse(pacienteData.GetString("id_pessoa")));
            enderecoData.Read();

            txtb_Nome.Text = pessoaData.GetString("nome");
            masktxtb_CGCCPF.Text = pessoaData.GetString("cgccpf");
            cbox_StatusPessoa.Text = pessoaData.GetString("fl_status");
            masktxtb_CEP.Text = enderecoData.GetString("CEP");
            cbox_UF.Text = enderecoData.GetString("uf");
            txtb_Cidade.Text = enderecoData.GetString("cidade");
            txtb_Bairro.Text = enderecoData.GetString("bairro");
            txtb_Rua.Text = enderecoData.GetString("rua");
            txtb_Numero.Text = enderecoData.GetString("numero");
            cbox_StatusPaciente.Text = pacienteData.GetString("fl_status");
            txtb_Risco.Text = pacienteData.GetString("paciente_risco");
            cbox_Obito.Text = pacienteData.GetString("fl_obito");

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Projeto Sistema Cadastro +Devs2Blu V1.0 \n-Alexandre Schanke da Costa");
        }

        private void masktxtb_CEP_TextChanged_1(object sender, EventArgs e)
        {
            //Validação do campo
            if (
                masktxtb_CEP.TextLength == 9 &&
                Regex.IsMatch(masktxtb_CEP.Text.Replace("-", ""), "^[0-9]{8}$") &&
                masktxtb_CEP.Text[5].Equals('-')
                )
            {
                searchCEP();
            }
        }

        #endregion

        #region HoverEvents
        private void ibtnCadastrar_MouseHover(object sender, EventArgs e)
        {
            ibtnCadastrar.BackColor = Color.DodgerBlue;
            ibtnCadastrar.ForeColor = Color.White;
            ibtnCadastrar.IconColor = Color.White;

        }
        private void ibtnCadastrar_MouseLeave(object sender, EventArgs e)
        {
            ibtnCadastrar.BackColor = Color.White;
            ibtnCadastrar.ForeColor = Color.Black;
            ibtnCadastrar.IconColor = Color.Black;
        }
        private void ibtnLimpar_MouseHover(object sender, EventArgs e)
        {

            ibtnLimpar.BackColor = Color.DodgerBlue;
            ibtnLimpar.ForeColor = Color.White;
            ibtnLimpar.IconColor = Color.White;
        }
        private void ibtnLimpar_MouseLeave(object sender, EventArgs e)
        {
            ibtnLimpar.BackColor = Color.White;
            ibtnLimpar.ForeColor = Color.Black;
            ibtnLimpar.IconColor = Color.Black;
        }
        private void ibtnInfo_MouseHover(object sender, EventArgs e)
        {
            ibtnInfo.BackColor = Color.DodgerBlue;
            ibtnInfo.ForeColor = Color.White;
            ibtnInfo.IconColor = Color.White;
        }

        private void ibtnInfo_MouseLeave(object sender, EventArgs e)
        {
            ibtnInfo.BackColor = Color.White;
            ibtnInfo.ForeColor = Color.Black;
            ibtnInfo.IconColor = Color.Black;
        }
        private void ibtnListar_MouseHover(object sender, EventArgs e)
        {
            ibtnListar.BackColor = Color.DodgerBlue;
            ibtnListar.ForeColor = Color.White;
            ibtnListar.IconColor = Color.White;
        }

        private void ibtnListar_MouseLeave(object sender, EventArgs e)
        {
            ibtnListar.BackColor = Color.White;
            ibtnListar.ForeColor = Color.Black;
            ibtnListar.IconColor = Color.Black;
        }
        #endregion
    }
}
