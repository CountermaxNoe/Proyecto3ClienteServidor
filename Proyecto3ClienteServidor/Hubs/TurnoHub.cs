using Microsoft.AspNetCore.SignalR;
using Proyecto3ClienteServidor.Services;

namespace Proyecto3ClienteServidor.Hubs
{
    public class TurnoHub : Hub
    {
        private readonly TurnoService _turnoService;

        public TurnoHub(TurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        public async Task SolicitarTurno()
        {
            var turno = _turnoService.SolicitarTurno(Context.ConnectionId);

            await Clients.Caller.SendAsync("TurnoAsignado", turno.Codigo, turno.Numero);

            await ActualizarFila();
        }

        public async Task LlamarSiguiente()
        {
            var turno = _turnoService.LlamarSiguiente();

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
            var fila = _turnoService.ObtenerFila();

            await Clients.All.SendAsync("ActualizarFila", fila.Select(t => new
            {
                codigo = t.Codigo,
                numero = t.Numero
            }));
        }
    }
}