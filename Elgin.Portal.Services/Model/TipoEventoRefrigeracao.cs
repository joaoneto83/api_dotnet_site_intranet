namespace Elgin.Portal.Services.Model
{
    public class TipoEventoRefrigeracao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public bool Ativo { get; set; }
        public int Ordem { get; set; }
    }
}
