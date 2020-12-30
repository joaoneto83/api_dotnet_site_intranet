using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public Guid IdPerfil { get; set; }
        public Guid? IdArquivo { get; set; }
        public bool ResetarSenha { get; set; }
        public string CodigoSenha { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public bool Ativo { get; set; }
        public bool? Selecionado { get; set; }
        public string NomePerfil { get; set; }
        public string NomeGrupos { get; set; }
        public string CaminhoFoto { get; set; }
        public string DataNascimento { get; set; }
        public string Telefone { get; set; }
        public Guid? IdSetor { get; set; }
        public string NomeSetor { get; set; }
        public string Registro { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Escolaridade { get; set; }
        public string EstadoCivil { get; set; }
        public string ComoSerChamado { get; set; }
        public string Celular { get; set; }
        public string ContatoEmergencia { get; set; }
        public string NomeContatoEmergencia { get; set; }
        public bool EmailAniversariante { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public Arquivo Arquivo { get; set; }
        public List<Dependente> Dependentes { get; set; }
        public List<string> Acessos { get; set; }

        public string Sigla
        {
            get
            {
                if (string.IsNullOrEmpty(Nome)) return "EL";

                var split = Nome.Split(" ");

                if (split.Length >= 2) return split[0][0].ToString().ToUpper() + split[1][0].ToString().ToUpper();

                if (Nome.Length >= 2) return Nome.Substring(0, 2).ToUpper();

                return "EL";
            }
        }
    }
}
