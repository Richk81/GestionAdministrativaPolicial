using Microsoft.AspNetCore.Mvc;

using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        //Constructor de la clase AccesoController con las dependencias necesarias
        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Método para manejar la solicitud de inicio de sesión (LOGIN)

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult RestablecerClave()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            // Validar campos obligatorios antes de consultar la base de datos
            if (string.IsNullOrWhiteSpace(modelo.Correo))
            {
                ViewData["Mensaje"] = "Debe ingresar un correo.";
                return View(modelo);
            }

            if (string.IsNullOrWhiteSpace(modelo.Clave))
            {
                ViewData["Mensaje"] = "Debe ingresar una contraseña.";
                return View(modelo);
            }

            // Intentar obtener al usuario con las credenciales
            Usuario usuario_encontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "Credenciales incorrectas";
                return View(modelo);
            }

            ViewData["Mensaje"] = null;

            // Crear las claims de manera segura, evitando nulls
            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario_encontrado.Nombre ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
        new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
        new Claim("UrlFoto", usuario_encontrado.UrlFoto ?? string.Empty),
        new Claim("Telefono", usuario_encontrado.Telefono ?? string.Empty),
        new Claim("Correo", usuario_encontrado.Correo ?? string.Empty)
    };

            // Crear el objeto ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Configurar las propiedades de autenticación
            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            // Iniciar sesión
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // Redirigir al Home después de un login exitoso
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool resultado = await _usuarioService.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo, se contraseña fue restablecida. Revise su E-mail";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["MensajeError"] = "Tenemos un problema!. Por favor inténtelo más tarde";
                    ViewData["Mensaje"] = null;
                }

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
            }
            return View();
        }
    }
}
