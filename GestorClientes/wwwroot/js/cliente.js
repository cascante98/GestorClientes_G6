(() => {

    const Cliente = {
        tabla: null,
        modalForm: null,
        modalDetalle: null,

        init() {
            this.modalForm = new bootstrap.Modal(document.getElementById('modalCliente'));
            this.modalDetalle = new bootstrap.Modal(document.getElementById('modalDetalle'));
            this.inicializarTabla();
            this.registrarEventos();
        },

        /** Escapa texto para evitar inyección de HTML al pintar datos. */
        escapar(texto) {
            return $('<div>').text(texto ?? '').html();
        },

        inicializarTabla() {
            this.tabla = new DataTable('#tblCliente', {
                ajax: { url: '/Cliente/ObtenerClientes', dataSrc: '', type: 'GET' },
                columns: [
                    { data: 'id' },
                    { data: 'cedula' },
                    {
                        data: null,
                        render: (c) => this.escapar(`${c.nombre} ${c.apellido1} ${c.apellido2}`.trim())
                    },
                    { data: 'correo', render: (d) => this.escapar(d) },
                    {
                        data: 'telefonos',
                        render: (t) => t.map(x => this.escapar(x.numero)).join('<br>')
                    },
                    {
                        data: 'id',
                        orderable: false,
                        render: (id) => `
                            <button class="btn-action btn-action-view" data-accion="ver" data-id="${id}" title="Ver detalle"><i class="bi bi-eye"></i></button>
                            <button class="btn-action btn-action-edit" data-accion="editar" data-id="${id}" title="Modificar"><i class="bi bi-pencil"></i></button>
                            <button class="btn-action btn-action-delete" data-accion="borrar" data-id="${id}" title="Borrar"><i class="bi bi-trash"></i></button>`
                    }
                ],
                language: { url: 'https://cdn.datatables.net/plug-ins/2.0.0/i18n/es-ES.json' }
            });
        },

        registrarEventos() {
            $('#btnNuevoCliente').on('click', () => this.abrirFormulario(null));
            $('#btnAgregarTelefono').on('click', () => this.agregarFilaTelefono());
            $('#formCliente').on('submit', (e) => { e.preventDefault(); this.guardar(); });

            $('#tblCliente tbody').on('click', 'button[data-accion]', (e) => {
                const boton = $(e.currentTarget);
                const id = boton.data('id');
                switch (boton.data('accion')) {
                    case 'ver': this.verDetalle(id); break;
                    case 'editar': this.editar(id); break;
                    case 'borrar': this.borrar(id); break;
                }
            });

            $('#listaTelefonos').on('click', '.btn-quitar-telefono', (e) => {
                $(e.currentTarget).closest('.fila-telefono').remove();
            });
        },

        mostrarMensaje(selector, texto, esCorrecto) {
            $(selector)
                .removeClass('d-none alert-success alert-danger')
                .addClass(esCorrecto ? 'alert-success' : 'alert-danger')
                .text(texto);
        },

        agregarFilaTelefono(telefono) {
            const fila = $(`
                <div class="row g-2 mb-2 fila-telefono">
                    <div class="col-6"><input class="form-control tel-numero" placeholder="Número" maxlength="20" /></div>
                    <div class="col-4">
                        <select class="form-select tel-tipo">
                            <option>Móvil</option><option>Casa</option><option>Trabajo</option><option>Otro</option>
                        </select>
                    </div>
                    <div class="col-2">
                        <button type="button" class="btn btn-outline-danger w-100 btn-quitar-telefono"><i class="bi bi-x-lg"></i></button>
                    </div>
                </div>`);
            if (telefono) {
                fila.find('.tel-numero').val(telefono.numero);
                fila.find('.tel-tipo').val(telefono.tipo || 'Otro');
            }
            $('#listaTelefonos').append(fila);
        },

        abrirFormulario(cliente) {
            $('#formCliente')[0].reset();
            $('#mensajeCliente').addClass('d-none');
            $('#listaTelefonos').empty();
            $('#clienteId').val(cliente ? cliente.id : 0);
            $('#modalClienteLabel').text(cliente ? 'Modificar cliente' : 'Registrar cliente');

            if (cliente) {
                $('#cedula').val(cliente.cedula);
                $('#nombre').val(cliente.nombre);
                $('#apellido1').val(cliente.apellido1);
                $('#apellido2').val(cliente.apellido2);
                $('#correo').val(cliente.correo);
                $('#direccion').val(cliente.direccion);
                cliente.telefonos.forEach(t => this.agregarFilaTelefono(t));
            }
            this.modalForm.show();
        },

        /** Arma el DTO a partir del formulario. */
        leerFormulario() {
            const telefonos = [];
            $('#listaTelefonos .fila-telefono').each(function () {
                const numero = $(this).find('.tel-numero').val().trim();
                if (numero) {
                    telefonos.push({ numero, tipo: $(this).find('.tel-tipo').val() });
                }
            });
            return {
                id: parseInt($('#clienteId').val(), 10) || 0,
                cedula: $('#cedula').val().trim(),
                nombre: $('#nombre').val().trim(),
                apellido1: $('#apellido1').val().trim(),
                apellido2: $('#apellido2').val().trim(),
                correo: $('#correo').val().trim(),
                direccion: $('#direccion').val().trim(),
                telefonos
            };
        },

        guardar() {
            const dto = this.leerFormulario();
            const esNuevo = dto.id === 0;
            const url = esNuevo ? '/Cliente/CrearCliente' : '/Cliente/ActualizarCliente';

            $.ajax({
                url,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(dto)
            }).done((respuesta) => {
                if (respuesta.esCorrecto) {
                    this.modalForm.hide();
                    this.tabla.ajax.reload();
                    this.mostrarMensaje('#mensajeGlobal', respuesta.mensaje, true);
                } else {
                    this.mostrarMensaje('#mensajeCliente', respuesta.mensaje, false);
                }
            }).fail(() => {
                this.mostrarMensaje('#mensajeCliente', 'Revise los datos obligatorios del formulario.', false);
            });
        },

        editar(id) {
            $.getJSON('/Cliente/ObtenerClientePorId', { id }).done((c) => this.abrirFormulario(c));
        },

        verDetalle(id) {
            $.getJSON('/Cliente/ObtenerClientePorId', { id }).done((c) => {
                const telefonos = c.telefonos.length
                    ? c.telefonos.map(t => `<li>${this.escapar(t.numero)} <span class="text-muted">(${this.escapar(t.tipo)})</span></li>`).join('')
                    : '<li class="text-muted">Sin teléfonos registrados</li>';
                $('#detalleCuerpo').html(`
                    <dl class="row mb-0">
                        <dt class="col-sm-4">Id</dt><dd class="col-sm-8">${c.id}</dd>
                        <dt class="col-sm-4">Cédula</dt><dd class="col-sm-8">${this.escapar(c.cedula)}</dd>
                        <dt class="col-sm-4">Nombre</dt><dd class="col-sm-8">${this.escapar(`${c.nombre} ${c.apellido1} ${c.apellido2}`.trim())}</dd>
                        <dt class="col-sm-4">Correo</dt><dd class="col-sm-8">${this.escapar(c.correo) || '-'}</dd>
                        <dt class="col-sm-4">Dirección</dt><dd class="col-sm-8">${this.escapar(c.direccion) || '-'}</dd>
                        <dt class="col-sm-4">Teléfonos</dt><dd class="col-sm-8"><ul class="mb-0 ps-3">${telefonos}</ul></dd>
                    </dl>`);
                this.modalDetalle.show();
            });
        },

        borrar(id) {
            if (!confirm('¿Desea borrar el cliente y sus teléfonos?')) {
                return;
            }
            $.post('/Cliente/EliminarCliente', { id }).done((respuesta) => {
                this.mostrarMensaje('#mensajeGlobal', respuesta.mensaje, respuesta.esCorrecto);
                this.tabla.ajax.reload();
            });
        }
    };

    $(document).ready(() => Cliente.init());

})();
