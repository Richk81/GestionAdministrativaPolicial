namespace GestionAdminPolicial.AplicacionWeb.Utilidades.Response
{
    //clase que se utiliza para encapsular las respuestas de las peticiones de nuestro sitio WEB
    //formato de respuesta que vamos a darle a cada una de las solicitudes que se haga a nuestro sitio web
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }
        public string? Mensaje { get; set; }
        public TObject? Objeto { get; set; }
        public List<TObject>? ListaObjeto { get; set; }
    }
}
