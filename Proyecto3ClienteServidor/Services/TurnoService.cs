using Proyecto3ClienteServidor.Models;

namespace Proyecto3ClienteServidor.Services
{
    public class TurnoService
    {
        private readonly Queue<TurnoModel> Fila = new();

        private int Contador = 0;

        private readonly object Lock = new();
        
        public TurnoModel SolicitarTurno(string connectionId)
        {
            lock (Lock) {

                Contador++;
                var turno = new TurnoModel
                {
                    Numero = Contador,
                    ConnectionId = connectionId
                };
                Fila.Enqueue(turno);
                return turno;
            }
        }
        public TurnoModel? LlamarSiguiente()
        {
            lock (Lock)
            {
                if (Fila.Count==0)
                     return null;

                return Fila.Dequeue();  


            }
           
        }
        public List<TurnoModel> ObtenerFila()
        {
            lock (Lock)
            {
                return Fila.ToList();
            }
        }
    }
}
