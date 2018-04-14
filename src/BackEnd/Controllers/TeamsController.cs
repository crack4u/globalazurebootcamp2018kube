using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class TeamsController : Controller
    {
        private readonly string _filePath;

        public TeamsController(IConfiguration configuration)
        {
            _filePath = configuration.GetValue<string>("OutputFilePath");
            if (!Path.IsPathRooted(_filePath))
            {
                _filePath = Path.Combine(Environment.CurrentDirectory, _filePath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            EnsureDirectoryExists();

            var teams = new List<Team>();

            string[] teamFiles = Directory.GetFiles(_filePath, "*.json");
            foreach (string teamFile in teamFiles)
            {
                string fileName = Path.Combine(_filePath, teamFile);
                Team team = JsonConvert.DeserializeObject<Team>(await System.IO.File.ReadAllTextAsync(fileName));
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

            Team team = JsonConvert.DeserializeObject<Team>(await System.IO.File.ReadAllTextAsync(fileName));
            return  Ok(team);
        }

        [HttpPut("{name}")]
        public async Task Put(string name, [FromBody]Team team)
        {
            EnsureDirectoryExists();

            string fileName = Path.Combine(_filePath, $"{name}.json");
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            team.Name = name;
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

    [DataContract]
    public class Team
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] Members { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public PowerGrid PowerGrid { get; set; }
    }

    [DataContract]
    public class PowerGrid
    {
        [DataMember]
        public int Intelligence { get; set; }

        [DataMember]
        public int Strength { get; set; }

        [DataMember]
        public int Speed { get; set; }

        [DataMember]
        public int Durability { get; set; }

        [DataMember]
        public int EnergyProjection { get; set; }

        [DataMember]
        public int FightingSkills { get; set; }
    }
}
