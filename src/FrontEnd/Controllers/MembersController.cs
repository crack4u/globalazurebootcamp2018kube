using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShieldHrm;
using static ShieldHrm.EmployeeServiceClient;

namespace FrontEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MembersController : Controller
    {
        private readonly IConfigurationRoot configuration;

        public MembersController(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        // GET: api/Members
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            string serviceUri = configuration.GetValue<string>("ShieldHrmEndpoint");
            EmployeeServiceClient client = new EmployeeServiceClient(
                EndpointConfiguration.BasicHttpBinding_IEmployeeService,
                $"{serviceUri}/EmployeeService.svc/EmployeeService");

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
