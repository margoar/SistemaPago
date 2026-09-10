using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaPago.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPago.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PaymentService _paymentService;

        public string Mensaje { get; set; }

        public IndexModel(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            Mensaje = _paymentService.ProcesarPago(100000);
        }
    }
}
