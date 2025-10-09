namespace GestionAdminPolicial.AplicacionWeb.Models
{
    /// <summary>
    /// Representa una respuesta genérica que contiene una lista de elementos.
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos de la lista.</typeparam>
    public class ResponseLista<T>
    {
        /// <summary>
        /// Lista de elementos del tipo especificado.
        /// </summary>
        public List<T> Data { get; set; }
    }
}

