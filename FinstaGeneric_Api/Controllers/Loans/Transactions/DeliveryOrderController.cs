using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinstaApi.Controllers.Loans.Transactions
{
    [Authorize]   
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class DeliveryOrderController : ControllerBase
    {
    }
}