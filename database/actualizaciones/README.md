# Actualizaciones manuales de base de datos

Cada cambio de esquema posterior a `BaseDatos.sql` se entregará como un archivo SQL numerado en esta carpeta. Los scripts deberán ejecutarse en orden sobre bases ya creadas.

Ejecuta [001_validaciones_clientes.sql](001_validaciones_clientes.sql) antes de probar el CRUD de Clientes sobre una base creada con una versión anterior del script. Incorpora las restricciones de identificación, género, dirección, teléfono y el valor predeterminado de estado.
