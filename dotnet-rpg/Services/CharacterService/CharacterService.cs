using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnet_rpg.Services.CharacterService {
    public class CharacterService : ICharacterService {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccesor) {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccesor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext
            .User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllUserCharacters() {

            var response = new ServiceResponse<List<GetCharacterDto>>();

            var id = GetUserId();

            var dbCharacters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == id)
                .ToListAsync();

            if (dbCharacters == null) {
                response.SetServiceResponse(false, "Not characters found.");
                return response;
            }

            var dtoCharacters = dbCharacters
                .Select(character => _mapper.Map<GetCharacterDto>(character)).ToList();

            response.Data = dtoCharacters;
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters() {

            var response = new ServiceResponse<List<GetCharacterDto>>();

            List<Character> dbCharacters = await GetCharacters();

            if (dbCharacters == null) {
                response.SetServiceResponse(false, "Not characters found.");
                return response;
            }

            var dtoCharacters = dbCharacters
                .Select(character => _mapper.Map<GetCharacterDto>(character)).ToList();

            response.Data = dtoCharacters;
            return response;
        }


        public async Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id) {

            var response = new ServiceResponse<GetCharacterDto>();

            var dbCharacter = await GetCharacterById(id);

            if (dbCharacter == null) response.SetServiceResponse(false, "Character not found.");
            else response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);

            return response;
        }

        public async Task<Character> GetCharacterById(int id) {

            var character = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id &&
                c.User.Id == GetUserId());

            return character;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter) {

            var response = new ServiceResponse<List<GetCharacterDto>>();

            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            var dbCharacters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
            response.Data = dbCharacters;

            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter) {

            var response = new ServiceResponse<GetCharacterDto>();

            try {
                var character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

                if (character.User.Id != GetUserId()) {
                    response.SetServiceResponse(false, "Character not found.");
                    return response;
                }

                /* _mapper.Map(updatedCharacter, character);*/

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strengh = updatedCharacter.Strengh;
                character.Defence = updatedCharacter.Defence;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id) {

            var response = new ServiceResponse<List<GetCharacterDto>>();

            try {
                var character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character == null) {
                    response.SetServiceResponse(false, "Character not found.");
                    return response;
                }

                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                var dbCharacters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();

                response.Data = dbCharacters;

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill) {

            var response = new ServiceResponse<GetCharacterDto>();

            try {

                var character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                    c.User.Id == GetUserId());

                if (character == null) {
                    response.SetServiceResponse(false, "Character not found.");
                    return response;
                }

                var skill = await _context.Skills
                    .FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

                if (skill == null) {
                    response.SetServiceResponse(false, "Skill not found.");
                    return response;
                }

                character.Skills.Add(skill);
                _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> ResetHP() {
            var response = new ServiceResponse<List<GetCharacterDto>>();

            var characters = await GetCharacters();
            if (characters == null) {
                response.SetServiceResponse(false, "Not characters found.");
                return response;
            }
            characters.ForEach(c => c.HitPoints = 1000);
            await _context.SaveChangesAsync();

            response.Data = characters
                .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        private async Task<List<Character>> GetCharacters() {
            return await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .ToListAsync();
        }

    }
}
