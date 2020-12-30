using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class EmailSettings
    {
        public String PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public String UsernameEmail { get; set; }
        public String UsernamePassword { get; set; }
        public String FromEmail { get; set; }
        public String ToEmail { get; set; }
        public String CcEmail { get; set; }
        public List<string> ContatoTo { get; set; }
        public List<string> SolarCco { get; set; }
        public List<string> ContatoAutomacaoTo { get; set; }
        public List<string> ContatoInformaticaTo { get; set; }
        public List<string> ContatoTelefonia { get; set; }
        public List<string> CredenciamentoTo { get; set; }
        public List<string> CredenciamentoIntegradorTo { get; set; }
        public List<string> SolicitacaoRhTo { get; set; }
        public List<string> SugestaoTiTo { get; set; }
        public List<string> LojaFuncionarioTo { get; set; }
        public List<string> ContatoGelattaTo { get; set; }
        public List<string> ContatoRefrigeracaoTo { get; set; }
        public List<string> QueroComprarTo { get; set; }
    }
}
