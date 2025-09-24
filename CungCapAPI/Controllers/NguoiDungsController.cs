using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CungCapAPI.Models.SqlServer;

namespace CungCapAPI.Controllers
{
    public class NguoiDungsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungsController(ApplicationDbContext context)
        {
            _context = context;
        }  
    }
}
