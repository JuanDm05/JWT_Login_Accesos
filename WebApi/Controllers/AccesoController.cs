using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.DTOS;
using WebApi.Models;
using WebApi.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaContext _dbpruebaContext;
        private readonly Utilidades _utilidades;

        public AccesoController(DbpruebaContext dbpruebaContext, Utilidades utilidades)
        {
            _dbpruebaContext = dbpruebaContext;
            _utilidades = utilidades;   
        }
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult>Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.encriptarSHA256(objeto.Clave)
            };

            await _dbpruebaContext.Usuarios.AddAsync(modeloUsuario);

            await _dbpruebaContext.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDTO obejto)
        {
            var usuarioEncontrado = await _dbpruebaContext.Usuarios
                .Where(u=>u.Correo==obejto.Correo&& u.Clave == _utilidades.encriptarSHA256(obejto.Clave)
                ).FirstOrDefaultAsync();

            if(usuarioEncontrado == null)
                return StatusCode(StatusCodes.Status200OK, new {isSucces= false, token=""});
            else
                return StatusCode(StatusCodes.Status200OK, new {isSucces= true, token=_utilidades.generarJWT(usuarioEncontrado)});
                
        }
    }
}
