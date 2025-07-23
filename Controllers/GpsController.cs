using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TowSat_Backend.Models;

namespace TowSat_Backend.Controllers {
    [Route("/gps")]
    [ApiController]
    [Authorize]
    public class GpsController : ControllerBase {

        private AppDbContext _dbcontext;

        public GpsController(AppDbContext context) {
            _dbcontext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GpsData>>> Get() {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }
            var userBtsData = await _dbcontext.GpsData
                                              .Where(b => b.UserId == userId)
                                              .ToListAsync();

            return Ok(userBtsData);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GpsData gpsDataToSave) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }

            gpsDataToSave.UserId = userId;

            _dbcontext.GpsData.Add(gpsDataToSave);
            await _dbcontext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = gpsDataToSave.Id }, gpsDataToSave);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }

            var gpsDataToDelete = await _dbcontext.GpsData.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (gpsDataToDelete == null) {
                return NotFound("Záznam nebyl nalezen nebo nemáte oprávnění k jeho smazání.");
            }

            _dbcontext.GpsData.Remove(gpsDataToDelete);
            await _dbcontext.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }

            var gpsRecordLoad = await _dbcontext.GpsData.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (gpsRecordLoad == null) {
                return NotFound("Záznam nebyl nalezen nebo nemáte oprávnění k jeho zobrazení.");
            }

            return Ok(gpsRecordLoad);
        }
    }
}
