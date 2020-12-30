namespace Elgin.Portal.Services.Model
{
    public class Cidade
    {
        public int Id { get; set; }
        public int IdEstado { get; set; }
        public string EstadoUf { get; set; }
        public string Descricao { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal SolarMediaAnual { get; set; }
    }
}
