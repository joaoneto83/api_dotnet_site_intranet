using System;
using System.Globalization;

namespace Elgin.Portal.Services.Model
{
    public class EventoRefrigeracao
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public DateTime DataDe { get; set; }
        public DateTime? DataAte { get; set; }
        public string DataFormatada
        {
            get
            {
                if (Cultura == "pt-Br" || Cultura == "es-ES")
                {
                    string strDe = Cultura == "pt-Br" ? "De " : "Del ";
                    string strA = Cultura == "pt-Br" ? " a " : " al ";

                    if (DataAte.HasValue && DataDe.Year != DataAte.Value.Year)// data completa até data completa
                    {
                        return strDe + DataDe.Day + " de " + DataDe.ToString("MMMM", new CultureInfo(Cultura)) + " de " + DataDe.Year + strA + DataAte.Value.Day + " de " + DataAte.Value.ToString("MMMM", new CultureInfo(Cultura)) + " de " + DataAte.Value.Year;
                    }
                    else
                    {
                        if (DataAte.HasValue && DataDe.Month != DataAte.Value.Month) // mesmo ano
                        {
                            return strDe + DataDe.Day + " de " + DataDe.ToString("MMMM", new CultureInfo(Cultura)) + strA + DataAte.Value.Day + " de " + DataAte.Value.ToString("MMMM", new CultureInfo(Cultura)) + " de " + DataDe.Year;
                        }
                        else
                        {
                            if (DataAte.HasValue && DataDe.Day != DataAte.Value.Day) // mesmo mês
                            {
                                return strDe + DataDe.Day + strA + DataAte.Value.Day + " de " + DataDe.ToString("MMMM", new CultureInfo(Cultura)) + " de " + DataDe.Year;
                            }
                            else // um dia de evento
                            {
                                return DataDe.Day + " de " + DataDe.ToString("MMMM", new CultureInfo(Cultura)) + " de " + DataDe.Year;
                            }
                        }
                    }
                }
                else
                {
                    if (DataAte.HasValue && DataDe.Year != DataAte.Value.Year)// data completa até data completa
                    {
                        return DataDe.ToString("MMMM dd, yyyy", new CultureInfo(Cultura)) + " to " + DataAte.Value.ToString("MMMM dd, yyyy", new CultureInfo(Cultura));
                    }
                    else
                    {
                        if (DataAte.HasValue && DataDe.Month != DataAte.Value.Month) // mesmo ano
                        {
                            return DataDe.ToString("MMMM ", new CultureInfo(Cultura)) + DataDe.Day + " to " + DataAte.Value.ToString("MMMM ", new CultureInfo(Cultura)) + DataAte.Value.Day + ", " + DataDe.Year;
                        }
                        else
                        {
                            if (DataAte.HasValue && DataDe.Day != DataAte.Value.Day) // mesmo mês
                            {
                                return DataDe.ToString("MMMM ", new CultureInfo(Cultura)) + DataDe.Day + " - " + DataAte.Value.Day + ", " + DataDe.Year;
                            }
                            else // um dia de evento
                            {
                                return DataDe.ToString("MMMM dd, yyyy", new CultureInfo(Cultura));
                            }
                        }
                    }
                }
            }
        }
        public string Local { get; set; }
        public string CaminhoImagem { get; set; }
        public int IdTipoEventoRefrigeracao { get; set; }
        public string NomeTipoEventoRefrigeracao { get; set; }
        public string Link { get; set; }
        public string Cultura { get; set; } = "pt-Br";
    }
}
