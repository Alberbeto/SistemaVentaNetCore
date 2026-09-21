namespace SistemaVentas_Aplicacion.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        public bool Estado { set; get; }

        public string? Mensaje { set; get; }

        public TObject Objeto { set; get; }
        
        public List<TObject> listaObjeto { set; get; }
    }
}
