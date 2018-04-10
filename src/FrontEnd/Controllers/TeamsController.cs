using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        private readonly HttpClient httpClient;

        public TeamsController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET: api/Teams
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            List<TeamDto> result = new List<TeamDto>();
            string proxyUrl = GetProxyUrl();

            using (HttpResponseMessage response = await httpClient.GetAsync(proxyUrl))
            {
                response.EnsureSuccessStatusCode();
                result.AddRange(JsonConvert.DeserializeObject<List<TeamDto>>(await response.Content.ReadAsStringAsync()));
            }

            return Json(result);
        }

        // PUT: api/Teams/name
        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody]string[] members)
        {
            string proxyUrl = GetProxyUrl(name);

            StringContent putContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    members,
                    score = -1
                }),
                Encoding.UTF8,
                "application/json");

            putContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (HttpResponseMessage response = await httpClient.PutAsync(proxyUrl, putContent))
            {
                return new ContentResult()
                {
                    StatusCode = (int)response.StatusCode,
                    Content = await response.Content.ReadAsStringAsync()
                };
            }
        }

        // DELETE: api/Teams/name
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            string proxyUrl = GetProxyUrl(name);

            using (HttpResponseMessage response = await httpClient.DeleteAsync(proxyUrl))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return StatusCode((int)response.StatusCode);
                }
            }

            return Ok();
        }

        private static string GetProxyUrl()
        {
            return $"http://TeamAssembler.Backend/api/Teams";
        }

        private static string GetProxyUrl(string teamName)
        {
            return $"http://TeamAssembler.Backend/api/Teams/{teamName}";
        }
    }

    public class TeamDto
    {
        public string Name { get; set; }

        public string[] Members { get; set; }

        public int Score { get; set; }
    }
}
