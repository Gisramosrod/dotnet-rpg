using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace dotnet_rpg.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase {

        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService) {
            _characterService = characterService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAllCharacters() {
            var response = await _characterService.GetAllCharacters();
            if (response == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("UserCharacters")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAllUserCharacters() {
            var response = await _characterService.GetAllUserCharacters();
            if (response == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetCharacter(int id) {
            var response = await _characterService.GetCharacter(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter) {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updatedCharacter) {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut("ResetHP")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> ResetHP() {
            var response = await _characterService.ResetHP();
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter(int id) {
            var response = await _characterService.DeleteCharacter(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill) {
            var response = await _characterService.AddCharacterSkill(newCharacterSkill);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

    }
}
