using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        private readonly string _filePath;

        public TeamsController()
        {
            _filePath = Path.Combine(Environment.CurrentDirectory, "teams");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            EnsureDirectoryExists();

            var teams = new List<TeamDto>();

            string[] teamFiles = Directory.GetFiles(_filePath, "*.json");
            foreach (string teamFile in teamFiles)
            {
                string fileName = Path.Combine(_filePath, teamFile);
                TeamDto team = JsonConvert.DeserializeObject<TeamDto>(await System.IO.File.ReadAllTextAsync(fileName));
                teams.Add(team);
            }

            return Json(teams);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            EnsureDirectoryExists();

            string fileName = Path.Combine(_filePath, $"{name}.json");
            if (!System.IO.File.Exists(fileName))
            {
                return NotFound();
            }

            TeamDto team = JsonConvert.DeserializeObject<TeamDto>(await System.IO.File.ReadAllTextAsync(fileName));
            return  Ok(team);
        }

        [HttpPut("{name}")]
        public async Task Put(string name, [FromBody]TeamDto team)
        {
            EnsureDirectoryExists();

            string fileName = Path.Combine(_filePath, $"{name}.json");
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            await System.IO.File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(team));
        }

        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            EnsureDirectoryExists();

            string fileName = Path.Combine(_filePath, $"{name}.json");
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
        }
    }

    public class TeamDto
    {
        public string Name { get; set; }

        public string[] Members { get; set; }

        public int Score { get; set; }
    }
}
