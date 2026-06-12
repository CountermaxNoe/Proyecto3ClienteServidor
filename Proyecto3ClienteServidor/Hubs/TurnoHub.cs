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

        public async Task SolicitarTurno(string clienteId)
        {
            var turno = _turnoService.SolicitarTurno(Context.ConnectionId, clienteId);
            var turno = turnoservice.SolicitarTurno(Context.ConnectionId);


            if (turno == null)
            {
                await Clients.Caller.SendAsync("RecepcionCerrada");
                return;
            }
            await Clients.Caller.SendAsync("TurnoAsignado", turno.Codigo, turno.Numero);
            await ActualizarFila();
        }
        public async Task ReconectarCliente(string clienteId)
        {
            var turno = _turnoService.ObtenerTurnoPorClienteId(clienteId, Context.ConnectionId);

            if (turno != null)
            {
                await Clients.Caller.SendAsync("TurnoRecuperado", turno.Codigo, turno.Numero);
                await ActualizarFila();
                return;
            }
            var turnoAtendido = _turnoService.ObtenerTurnoAtendidoPorClienteId(clienteId);

            if (turnoAtendido != null)
            {
                await Clients.Caller.SendAsync("TurnoYaPaso", turnoAtendido.Codigo);
            }
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
        public async Task CancelarTurno()
        {
            var cancelado = _turnoService.CancelarTurno(Context.ConnectionId);

            if (cancelado)
            {
                await Clients.Caller.SendAsync("TurnoCancelado");
                await ActualizarFila();
            }
            else
            {
                await Clients.Caller.SendAsync("NoTieneTurno");
            }
        }
        public async Task CerrarRecepcion()
        {
            _turnoService.CerrarRecepcion();
            await Clients.All.SendAsync("EstadoRecepcion", false);
        }
        public async Task AbrirRecepcion()
        {
            _turnoService.AbrirRecepcion();
            await Clients.All.SendAsync("EstadoRecepcion", true);
        }
        public async Task ResetearSistema()
        {
            _turnoService.ResetearSistema();

            await Clients.All.SendAsync("SistemaReseteado");
            await Clients.All.SendAsync("EstadoRecepcion", true);
            await Clients.All.SendAsync("TurnoActual", "---");

            await ActualizarFila();
        }
        public override async Task OnConnectedAsync()
        {
            var total = _turnoService.RegistrarClienteConectado(Context.ConnectionId);

            await Clients.All.SendAsync("ClientesConectados", total);

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var total = _turnoService.RegistrarClienteDesconectado(Context.ConnectionId);
            await Clients.All.SendAsync("ClientesConectados", total);
            await base.OnDisconnectedAsync(exception);

        }
    }
}