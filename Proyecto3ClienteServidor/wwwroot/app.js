let clienteId = localStorage.getItem("clienteId");
if (!clienteId) {
    clienteId = crypto.randomUUID();
    localStorage.setItem("clienteId", clienteId);
}
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/turnoHub")
    .build();

connection.start()
    .then(() => {
        console.log("Conectado a SignalR")
        connection.invoke("ReconectarCliente", clienteId);
    })
    .catch(err => console.error(err));
function cerrarRecepcion() {
    connection.invoke("CerrarRecepcion");
}
function abrirRecepcion() {

    connection.invoke("AbrirRecepcion");
}

function solicitarTurno() {
    connection.invoke("SolicitarTurno", clienteId);
}
function cancelarTurno() {
    connection.invoke("CancelarTurno");

}
function LlamarSiguiente() {
    connection.invoke("LlamarSiguiente");
}
function resetearSistema() {

    connection.invoke("ResetearSistema");
}
connection.on("RecepcionCerrada", () => {
    document.getElementById("mensajeCliente").innerText = "La recepcion esta cerrada. no se pueden solicitar turnos nuevos.";


});
connection.on("EstadoRecepcion", (abierta) => {
    const estado = document.getElementById("estadoRecepcion");
    const botonSolicitar = document.querySelector("button[onclick='solicitarTurno()']");

    if (abierta) {
        estado.innerText = "Recepcion abierta";
        botonSolicitar.disabled = false;
    }
    else {
        estado.innerText = "Recepcion cerrada";
        botonSolicitar.disabled = true;
    }
});
connection.on("TurnoAsignado", (codigo, numero) => {
    document.getElementById("miTurno").innerText = codigo;
    document.getElementById("mensajeCliente").innerText =
        "Tu turno fue registrado correctamente.";
});

connection.on("TurnoLlamado", (codigo) => {
    document.getElementById("miTurno").innerText = codigo;

    document.getElementById("mensajeCliente").innerText =
        `¡Es tu turno ${codigo}! Pasa a ventanilla.`;

    alert(`¡Es tu turno ${codigo}!`);
});

connection.on("TurnoCancelado", () => {
    document.getElementById("miTurno").innerText = "___";
    document.getElementById("mensajeCliente").innerText = "Tu turno fue cancelado correctamente."
});

connection.on("NoTieneTurno", () => {
    document.getElementById("mensajeCliente").innerText = "No tienes turno activo para cancelar."
});

connection.on("TurnoActual", (codigo) => {
    document.getElementById("turnoActual").innerText = codigo;
});
connection.on("TurnoYaPaso", (codigo) => {
    document.getElementById("miTurno").innerText = codigo;


    document.getElementById("mensajeCliente").innerText =
        `Tu turno ${codigo} ya fue llamado mientras estabas desconectado.`;

    alert(`Tu turno ${codigo} ya pasó.`);
});
connection.on("TurnoRecuperado", (codigo, numero) => {
    document.getElementById("miTurno").innerText = codigo;

    document.getElementById("mensajeCliente").innerText = `Ya tenías el turno ${codigo} asignado.`;

});
connection.on("SinTurnos", () => {
    document.getElementById("turnoActual").innerText = "___";

    alert("No hay turnos en espera.");
});
connection.on("ClientesConectados", (total) => {
    document.getElementById("clientesConectados").innerText =
        `Clientes conectados: ${total}`;
});
connection.on("SistemaReseteado", () => {
    document.getElementById("miTurno").innerText = "___";
    document.getElementById("turnoActual").innerText = "___";
    document.getElementById("mensajeCliente").innerText =
        "El sistema fue reiniciado. puedes solicitar un nuevo turno";
});







connection.on("ActualizarFila", (fila) => {
    const lista = document.getElementById("fila");
    lista.innerHTML = "";
    document.getElementById("contadorFila").innerText =
        `Personas en espera: ${fila.length}`;

    fila.forEach((turno, index) => {
        const li = document.createElement("li");
        li.innerText =
            `Turno: ${turno.codigo} | Personas antes: ${index}`;
        lista.appendChild(li);
    });
});