namespace SistemaVentas_Aplicacion.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        public bool? Estado { get; set; }

        public string? Mensaje { get; set; }

        public TObject Objeto { get; set; }

        public List<TObject> listaObjeto { get; set; }
    }
}
