using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TowSat_Backend.Models;

namespace TowSat_Backend.Controllers {
    [Route("/bts")]
    [ApiController]
    [Authorize]
    public class BtsController : ControllerBase {

        private AppDbContext _dbcontext;

        public BtsController(AppDbContext context) {
            _dbcontext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BtsData>>> Get() {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) {                
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }
            var userBtsData = await _dbcontext.BtsData
                                              .Where(b => b.UserId == userId)
                                              .ToListAsync();

            return Ok(userBtsData);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BtsData btsDataToSave) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }
                       
            btsDataToSave.UserId = userId;

            _dbcontext.BtsData.Add(btsDataToSave);
            await _dbcontext.SaveChangesAsync(); 
            
            return CreatedAtAction(nameof(Get), new { id = btsDataToSave.Id }, btsDataToSave);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }
           
            var btsDataToDelete = await _dbcontext.BtsData.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (btsDataToDelete == null) {               
                return NotFound("Záznam nebyl nalezen nebo nemáte oprávnění k jeho smazání.");
            }

            _dbcontext.BtsData.Remove(btsDataToDelete);
            await _dbcontext.SaveChangesAsync(); 
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized("Uživatel není přihlášen nebo jeho ID nebylo nalezeno.");
            }
            
            var btsRecordLoad = await _dbcontext.BtsData.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (btsRecordLoad == null) {               
                return NotFound("Záznam nebyl nalezen nebo nemáte oprávnění k jeho zobrazení.");
            }

            return Ok(btsRecordLoad);
        }
    }
}
