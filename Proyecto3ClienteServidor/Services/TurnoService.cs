using Proyecto3ClienteServidor.Models;

namespace Proyecto3ClienteServidor.Services
{
    public class TurnoService
    {
        private readonly Queue<TurnoModel> Fila = new();

        private int Contador = 0;
        private readonly Dictionary<string, TurnoModel> TurnosAtendidos = new();
        public bool RecepcionAbierta { get; set; } = true;
        private readonly HashSet<string> ClientesConectados = new();
        private readonly object Lock = new();


        public TurnoModel? SolicitarTurno(string connectionId, string clienteId)
        {


            lock (Lock)
            {
                if (!RecepcionAbierta)
                    return null;
                var existente = Fila.FirstOrDefault(x => x.ClienteId == clienteId);

                if (existente != null)
                {
                    existente.ConnectionId = connectionId;
                    return existente;
                }
                Contador++;

                var turno = new TurnoModel
                {
                    Numero = Contador,
                    ConnectionId = connectionId,
                    ClienteId = clienteId,
                };
                Fila.Enqueue(turno);
                return turno;
            }
        }
        public int RegistrarClienteConectado(string connetionId)
        {
            lock (Lock)
            {
                ClientesConectados.Add(connetionId);
                return ClientesConectados.Count;    
            }
        }
        public int RegistrarClienteDesconectado(string connetionId)
        {
            lock (Lock)
            {
                ClientesConectados.Remove(connetionId);
                return ClientesConectados.Count;
            }
        }   
        public int ObtenerClientesConectados()
        {
            lock (Lock)
            {
                return ClientesConectados.Count;
            }
        }       
        public TurnoModel? ObtenerTurnoPorClienteId(string clienteId, string connectionId)
        {
            lock (Lock)
            {
                var turno = Fila.FirstOrDefault(t => t.ClienteId == clienteId);

                if (turno != null)
                {
                    turno.ConnectionId = connectionId;
                }

                return turno;
            }
        }
        public TurnoModel? LlamarSiguiente()
        {
            lock (Lock)
            {
                if (Fila.Count == 0)
                    return null;

                var turno = Fila.Dequeue();

                if (!string.IsNullOrEmpty(turno.ClienteId))
                {
                    TurnosAtendidos[turno.ClienteId] = turno;
                }
                return turno;

            }


        }
        public TurnoModel? ObtenerTurnoAtendidoPorClienteId(string clienteId)
        {
            lock (Lock)
            {
                if (TurnosAtendidos.ContainsKey(clienteId))
                {
                    return TurnosAtendidos[clienteId];
                }
                return null;
            }
        }
        public List<TurnoModel> ObtenerFila()
        {
            lock (Lock)
            {
                return Fila.ToList();
            }
        }
        public bool CancelarTurno(string connectionId)
        {
            lock (Lock)
            {
                var lista = Fila.ToList();

                var turno = lista.FirstOrDefault(x => x.ConnectionId == connectionId);

                if (turno == null)
                    return false;
                lista.Remove(turno);

                Fila.Clear();

                foreach (var item in lista)
                {
                    Fila.Enqueue(item);
                }
                return true;
            }
        }
        public void CerrarRecepcion()
        {
            lock (Lock)
            {
                RecepcionAbierta = false;
            }
        }
        public void AbrirRecepcion()
        {
            lock (Lock)
            {
                RecepcionAbierta = true;
            }
        }
        public bool EstaRecepcionAbierta()
        {
            lock (Lock)
            {
                return RecepcionAbierta;
            }
        }   
    
    public void ResetearSistema()

        {
            lock (Lock)
            {
                Fila.Clear();
                TurnosAtendidos.Clear();
                Contador = 0;
                RecepcionAbierta = true;


            }
        }
    }
}