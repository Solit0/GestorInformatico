function limpiarTodosLosErrores() {
    document.querySelectorAll('[id^="error"], [id$="Errores"]').forEach(function (el) {
        if (el.id.indexOf('Resumen') > -1) { el.style.display = 'none'; el.innerHTML = ''; }
        else { el.style.display = 'none'; el.textContent = ''; }
    });
    document.querySelectorAll('.is-invalid').forEach(function (el) { el.classList.remove('is-invalid'); });
}

function marcarCampo(inputId, errorId, msg) {
    var input = document.getElementById(inputId);
    var error = document.getElementById(errorId);
    if (input) input.classList.add('is-invalid');
    if (error) { error.style.display = 'block'; error.textContent = msg; }
}

function esEmail(v) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v); }
function esDUI(v) { return /^\d{8}-\d$/.test(v); }
function esTel(v) { return /^[0-9]{4}-?[0-9]{4}$/.test(v); }

function validarCliente(e) {
    e.preventDefault();
    limpiarTodosLosErrores();
    var errores = [];
    var nombre = document.getElementById('clienteNombre').value.trim();
    var dui = document.getElementById('clienteDUI').value.trim();
    var tel = document.getElementById('clienteTelefono').value.trim();
    var email = document.getElementById('clienteEmail').value.trim();
    var dir = document.getElementById('clienteDireccion').value.trim();

    if (!nombre) { marcarCampo('clienteNombre', 'errorClienteNombre', 'El nombre es obligatorio'); errores.push('Nombre'); }
    if (!dui) { marcarCampo('clienteDUI', 'errorClienteDUI', 'El DUI es obligatorio'); errores.push('DUI'); }
    else if (!esDUI(dui)) { marcarCampo('clienteDUI', 'errorClienteDUI', 'Formato inv\u00e1lido. Debe ser 12345678-9 (8 d\u00edgitos, guion, 1 d\u00edgito)'); errores.push('DUI'); }
    if (!tel) { marcarCampo('clienteTelefono', 'errorClienteTelefono', 'El tel\u00e9fono es obligatorio'); errores.push('Tel\u00e9fono'); }
    else if (!esTel(tel)) { marcarCampo('clienteTelefono', 'errorClienteTelefono', 'Formato inv\u00e1lido. Debe ser 2222-3333 o 22223333'); errores.push('Tel\u00e9fono'); }
    if (!email) { marcarCampo('clienteEmail', 'errorClienteEmail', 'El correo es obligatorio'); errores.push('Correo'); }
    else if (!esEmail(email)) { marcarCampo('clienteEmail', 'errorClienteEmail', 'Formato de correo inv\u00e1lido'); errores.push('Correo'); }
    if (!dir) { marcarCampo('clienteDireccion', 'errorClienteDireccion', 'La direcci\u00f3n es obligatoria'); errores.push('Direcci\u00f3n'); }

    if (errores.length > 0) {
        var resumen = document.getElementById('clienteResumenErrores');
        resumen.innerHTML = '<i class="bi bi-exclamation-triangle me-1"></i>Corrija: ' + errores.join(', ');
        resumen.style.display = 'block';
        return;
    }
    e.target.submit();
}

function validarEquipo(e) {
    e.preventDefault();
    limpiarTodosLosErrores();
    var errores = [];
    var cliente = document.getElementById('equipoClienteId').value;
    var nombre = document.getElementById('equipoNombre').value.trim();
    var marca = document.getElementById('equipoMarca').value.trim();
    var modelo = document.getElementById('equipoModelo').value.trim();
    var serie = document.getElementById('equipoSerie').value.trim();

    if (!cliente) { marcarCampo('equipoClienteId', 'errorEquipoClienteId', 'Seleccione un cliente'); errores.push('Cliente'); }
    if (!nombre) { marcarCampo('equipoNombre', 'errorEquipoNombre', 'El nombre es obligatorio'); errores.push('Nombre'); }
    if (!marca) { marcarCampo('equipoMarca', 'errorEquipoMarca', 'La marca es obligatoria'); errores.push('Marca'); }
    if (!modelo) { marcarCampo('equipoModelo', 'errorEquipoModelo', 'El modelo es obligatorio'); errores.push('Modelo'); }
    if (!serie) { marcarCampo('equipoSerie', 'errorEquipoSerie', 'El n\u00famero de serie es obligatorio'); errores.push('Serie'); }

    if (errores.length > 0) {
        var resumen = document.getElementById('equipoResumenErrores');
        resumen.innerHTML = '<i class="bi bi-exclamation-triangle me-1"></i>Corrija: ' + errores.join(', ');
        resumen.style.display = 'block';
        return;
    }
    e.target.submit();
}

document.addEventListener('DOMContentLoaded', function () {
    var fc = document.getElementById('formCliente');
    if (fc) fc.addEventListener('submit', validarCliente);

    var fe = document.getElementById('formEquipo');
    if (fe) fe.addEventListener('submit', validarEquipo);

    document.querySelectorAll('#formCliente input, #formEquipo input, #formEquipo select').forEach(function (el) {
        el.addEventListener('input', function () {
            this.classList.remove('is-invalid');
            var err = this.parentElement.querySelector('[id^="error"]');
            if (err) { err.style.display = 'none'; err.textContent = ''; }
        });
    });
});
