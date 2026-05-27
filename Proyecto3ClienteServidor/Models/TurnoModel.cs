namespace Proyecto3ClienteServidor.Models
{
    public class TurnoModel
    {
        public int Numero { get; set; }

        public string Codigo => $"A-{Numero:000}";

        public string ConnectionId { get; set; } = "";

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    }
}
