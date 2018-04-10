using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShieldHrm;
using static ShieldHrm.EmployeeServiceClient;

namespace FrontEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MembersController : Controller
    {
        public MembersController()
        {
        }

        // GET: api/Members
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            EmployeeServiceClient client = new EmployeeServiceClient(EndpointConfiguration.BasicHttpBinding_IEmployeeService,  "http://51.136.9.144:83/EmployeeService.svc/EmployeeService");

            try
            {
                return Json(await client.GetEmployeeListAsync());
            }
            finally
            {
                await client.CloseAsync();
            }
        }
    }
}
