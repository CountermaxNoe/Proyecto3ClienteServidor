const connection = new signalR.HubConnectionBuilder()
    .withUrl("/turnoHub")
    .build();

connection.start()
    .then(() => console.log("Conectado a SignalR"))
    .catch(err => console.error(err));

function solicitarTurno() {
    connection.invoke("SolicitarTurno");
}

function LlamarSiguiente() {
    connection.invoke("LlamarSiguiente");
}

connection.on("TurnoAsignado", (codigo, numero) => {
    document.getElementById("miTurno").innerText = codigo;
    document.getElementById("mensajeCliente").innerText =
        "Tu turno fue registrado correctamente.";
});

connection.on("TurnoLlamado", (codigo) => {
    document.getElementById("mensajeCliente").innerText =
        `¡Es tu turno ${codigo}! Pasa a ventanilla.`;

    alert(`¡Es tu turno ${codigo}!`);
});

connection.on("TurnoActual", (codigo) => {
    document.getElementById("turnoActual").innerText = codigo;
});

connection.on("SinTurnos", () => {
    alert("No hay turnos en espera.");
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