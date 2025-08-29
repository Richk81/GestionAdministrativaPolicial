using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.BLL.Interfaces;
using System.Security.Cryptography;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0,6); // Genera un GUID y toma los primeros 6 caracteres
            return clave;
        }
        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(texto)); //convierte el texto a bytes y lo encripta

                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2")); //convierte el byte a hexadecimal
                }
            }
            return sb.ToString(); //devuelve el texto encriptado
        }
        // // 🔧 IMPORTANTE: todo lo anterior devuelve la clave o contraseña del usuario encriptada,
        // para poder guardarla como corresponde en la base de datos

    }
}
