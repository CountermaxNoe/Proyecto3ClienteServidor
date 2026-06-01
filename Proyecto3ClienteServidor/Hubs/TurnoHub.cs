using Microsoft.AspNetCore.SignalR;
using Proyecto3ClienteServidor.Services;

namespace Proyecto3ClienteServidor.Hubs
{
    public class TurnoHub : Hub
    {
        private readonly TurnoService turnoservice;

        public TurnoHub(TurnoService turnoService)
        {
            this.turnoservice = turnoService;
        }

        public async Task SolicitarTurno()
        {
            var turno = turnoservice.SolicitarTurno(Context.ConnectionId);

            await Clients.Caller.SendAsync("TurnoAsignado", turno.Codigo, turno.Numero);

            await ActualizarFila();
        }

        public async Task LlamarSiguiente()
        {
            var turno = turnoservice.LlamarSiguiente();

            if (turno == null)
            {
                await Clients.Caller.SendAsync("SinTurnos");
                return;
            }

            await Clients.Client(turno.ConnectionId).SendAsync("TurnoLlamado", turno.Codigo);

            await Clients.All.SendAsync("TurnoActual", turno.Codigo);

            await ActualizarFila();
        }

        private async Task ActualizarFila()
        {
            var fila = turnoservice.ObtenerFila();

            await Clients.All.SendAsync("ActualizarFila", fila.Select(t => new
            {
                codigo = t.Codigo,
                numero = t.Numero
            }));
        }
    }
}