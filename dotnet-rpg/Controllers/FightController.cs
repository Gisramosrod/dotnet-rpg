using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.FightService;
using dotnet_rpg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace dotnet_rpg.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FightController : ControllerBase {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService) {
            _fightService = fightService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request) {
            return Ok(await _fightService.Fight(request));
        }

        [HttpGet("Hightscores")]
        public async Task<ActionResult<ServiceResponse<List<HightscoreDto>>>> GetHightscore() {
            return Ok(await _fightService.GetHightscore());
        }

        [Authorize]
        [HttpPost("WeaponAttackRandomOpponent")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttackRandom(WeaponAttackRandomDto request) {
            return Ok(await _fightService.WeaponAttackRandomOpponent(request));
        }

        [Authorize]
        [HttpPost("SkillAttackRandomOpponent")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttackRandom(SkillAttackRandomDto request) {
            return Ok(await _fightService.SkillAttackRandomOpponent(request));
        }

        [HttpPost("WeaponAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request) {
            return Ok(await _fightService.WeaponAttackOpponent(request));
        }

        [HttpPost("SkillAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request) {
            return Ok(await _fightService.SkillAttackOpponent(request));
        }        
    }
}
