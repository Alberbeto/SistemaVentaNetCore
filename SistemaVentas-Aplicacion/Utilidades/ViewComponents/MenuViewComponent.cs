using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.BLL.Interfaces;
using SistemaVentas_Aplicacion.Models.VModels;
using System.Security.Claims;

namespace SistemaVentas_Aplicacion.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuService menuService,
            IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claims = HttpContext.User;

            List<VMMenu> listaMenus;

            if (claims.Identity.IsAuthenticated)
            {
                string idUsuario = claims.Claims
             .Where(c => c.Type == ClaimTypes.NameIdentifier)
             .Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<VMMenu>>(await _menuService.obtenerMenu(int.Parse(idUsuario)));
            }
            else
            {
                listaMenus = new List<VMMenu> { };
            }

            return View(listaMenus);
        }
    }
}
