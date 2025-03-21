using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;

namespace SistemaPermisos.Controllers
{
    public class OmisionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OmisionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Omisiones
        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rol = HttpContext.Session.GetString("UsuarioRol");

            // Si es administrador, mostrar todas las omisiones
            if (rol == "Admin")
            {
                var omisiones = await _context.OmisionesMarca
                    .Include(o => o.Usuario)
                    .OrderByDescending(o => o.FechaRegistro)
                    .ToListAsync();
                return View(omisiones);
            }

            // Si es docente, mostrar solo sus omisiones
            var misOmisiones = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaRegistro)
                .ToListAsync();

            return View(misOmisiones);
        }

        // GET: Omisiones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (omision == null)
            {
                return NotFound();
            }

            // Verificar que el usuario tenga acceso a esta omisión
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (omision.UsuarioId != usuarioId && rol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(omision);
        }

        // GET: Omisiones/Create
        public IActionResult Create()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener información del usuario para prellenar el formulario
            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario != null)
            {
                var viewModel = new OmisionViewModel
                {
                    FechaOmision = DateTime.Now,
                    Cedula = usuario.Cedula ?? "",
                    Puesto = usuario.Puesto ?? ""
                };
                return View(viewModel);
            }

            return View(new OmisionViewModel { FechaOmision = DateTime.Now });
        }

        // POST: Omisiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OmisionViewModel model)
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var omision = new OmisionMarca
                {
                    UsuarioId = usuarioId.Value,
                    FechaOmision = model.FechaOmision,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Instancia = model.Instancia,
                    CategoriaPersonal = model.CategoriaPersonal,
                    Titulo = model.Titulo,
                    TipoOmision = model.TipoOmision,
                    Motivo = model.Motivo,
                    Estado = "Pendiente",
                    FechaRegistro = DateTime.Now
                };

                // Actualizar información del usuario si es necesario
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario != null)
                {
                    if (string.IsNullOrEmpty(usuario.Cedula))
                    {
                        usuario.Cedula = model.Cedula;
                    }
                    if (string.IsNullOrEmpty(usuario.Puesto))
                    {
                        usuario.Puesto = model.Puesto;
                    }
                    _context.Update(usuario);
                }

                _context.Add(omision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Omisiones/Resolve/5 (Solo para administradores)
        public async Task<IActionResult> Resolve(int? id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (omision == null)
            {
                return NotFound();
            }

            var viewModel = new ResolucionOmisionViewModel
            {
                OmisionId = omision.Id
            };

            ViewBag.Omision = omision;

            return View(viewModel);
        }

        // POST: Omisiones/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(ResolucionOmisionViewModel model)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var omision = await _context.OmisionesMarca.FindAsync(model.OmisionId);
                if (omision == null)
                {
                    return NotFound();
                }

                omision.Resolucion = model.Resolucion;
                omision.ObservacionesResolucion = model.ObservacionesResolucion;

                // Actualizar el estado según la resolución
                if (model.Resolucion.StartsWith("Aceptar") || model.Resolucion == "Acoger convocatoria")
                {
                    omision.Estado = "Aprobado";
                }
                else if (model.Resolucion == "Denegar lo solicitado")
                {
                    omision.Estado = "Rechazado";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, volver a cargar la omisión para la vista
            var omisionDb = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == model.OmisionId);

            ViewBag.Omision = omisionDb;

            return View(model);
        }

        // GET: Omisiones/Approve/5 (Solo para administradores) - Método obsoleto, usar Resolve
        public async Task<IActionResult> Approve(int? id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca.FindAsync(id);
            if (omision == null)
            {
                return NotFound();
            }

            omision.Estado = "Aprobado";
            omision.Resolucion = "Aceptar sin rebajo salarial";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Omisiones/Reject/5 (Solo para administradores) - Método obsoleto, usar Resolve
        public async Task<IActionResult> Reject(int? id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca.FindAsync(id);
            if (omision == null)
            {
                return NotFound();
            }

            omision.Estado = "Rechazado";
            omision.Resolucion = "Denegar lo solicitado";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

