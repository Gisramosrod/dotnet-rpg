using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnet_rpg.Services.FightService {
    public class FightService : IFightService {
        private readonly ICharacterService _characterService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FightService(ICharacterService characterService, DataContext context,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) {
            _characterService = characterService;
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext
            .User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request) {

            var response = new ServiceResponse<FightResultDto>() {
                Data = new FightResultDto()
            };

            try {

                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated) {

                    foreach (Character attacker in characters) {

                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();

                        int indexOpponent = new Random().Next(opponents.Count);
                        var opponent = opponents[indexOpponent];

                        int damage = 0;
                        string attackeUsed = string.Empty;

                        if (attacker.Weapon == null) {
                            int indexSkill = new Random().Next(attacker.Skills.Count);
                            var skill = attacker.Skills[indexSkill];
                            attackeUsed = skill.Name;
                            damage = GetDamageWithSkill(attacker, opponent, skill);

                        } else if (attacker.Skills.Count == 0) {
                            attackeUsed = attacker.Weapon.Name;
                            damage = GetDamageWithWeapon(attacker, opponent);

                        } else {

                            bool useWeapon = new Random().Next(2) == 0;
                            if (useWeapon) {
                                attackeUsed = attacker.Weapon.Name;
                                damage = GetDamageWithWeapon(attacker, opponent);

                            } else {
                                int indexSkill = new Random().Next(attacker.Skills.Count);
                                var skill = attacker.Skills[indexSkill];
                                attackeUsed = skill.Name;
                                damage = GetDamageWithSkill(attacker, opponent, skill);

                            }
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attack {opponent.Name} using {attackeUsed} dealing {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HitPoints <= 0) {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left");
                            break; //como q no creo q haga falta si no para q pusimos el defeated
                        }
                    }
                }
                characters.ForEach(c => {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttackOpponent(WeaponAttackDto request) {

            var response = new ServiceResponse<AttackResultDto>();

            try {

                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = GetDamageWithWeapon(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttackOpponent(SkillAttackDto request) {

            var response = new ServiceResponse<AttackResultDto>();

            try {

                var attacker = await _context.Characters
                  .Include(c => c.Skills)
                  .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill == null) {
                    response.SetServiceResponse(false, $"{attacker.Name} doesn't know that skill.");
                    return response;
                }

                int damage = GetDamageWithSkill(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttackRandomOpponent(WeaponAttackRandomDto request) {

            var response = new ServiceResponse<AttackResultDto>();

            try {

                var attacker = await _characterService.GetCharacterById(request.AttackerId);

                if (attacker == null) {
                    response.SetServiceResponse(false, "Character not found.");
                    return response;
                }

                if (attacker.Weapon == null) {
                    response.SetServiceResponse(false, $"{attacker.Name} doesn't have a weapon!");
                    return response;
                }

                var posibleOpponents = await GetOpponentCharacters();

                if (posibleOpponents == null) {
                    response.SetServiceResponse(false, $"There are no opponents for {attacker.Name}");
                    return response;
                }

                int randomIndexOpponent = new Random().Next(posibleOpponents.Count());
                var opponent = posibleOpponents[randomIndexOpponent];

                int damage = GetDamageWithWeapon(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttackRandomOpponent(SkillAttackRandomDto request) {
          
            var response = new ServiceResponse<AttackResultDto>();

            try {

                var attacker = await _characterService.GetCharacterById(request.AttackerId);

                if (attacker == null) {
                    response.SetServiceResponse(false, "Character not found.");
                    return response;
                }

                if (attacker.Skills.Count == 0) {
                    response.SetServiceResponse(false, $"{attacker.Name} doesn't have skills.");
                    return response;
                }

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill == null) {
                    response.SetServiceResponse(false, $"{attacker.Name} doesn't know that skill.");
                    return response;
                }

                var posibleOpponents = await GetOpponentCharacters();

                if (posibleOpponents == null) {
                    response.SetServiceResponse(false, $"There are no opponents for {attacker.Name}");
                    return response;
                }

                int randomIndexOpponent = new Random().Next(posibleOpponents.Count());
                var opponent = posibleOpponents[randomIndexOpponent];

                int damage = GetDamageWithSkill(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            } catch (Exception ex) { response.SetServiceResponse(false, ex.Message); }

            return response;
        }

        public async Task<ServiceResponse<List<HightscoreDto>>> GetHightscore() {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            var response = new ServiceResponse<List<HightscoreDto>>() {
                Data = characters.Select(c => _mapper.Map<HightscoreDto>(c)).ToList()
            };

            return response;
        }

        private static int GetDamageWithWeapon(Character? attacker, Character? opponent) {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strengh));
            damage -= new Random().Next(opponent.Strengh);

            if (damage > 0) opponent.HitPoints -= damage;
            return damage;
        }

        private static int GetDamageWithSkill(Character? attacker, Character? opponent, Skill? skill) {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Strengh);

            if (damage > 0) opponent.HitPoints -= damage;
            return damage;
        }

        private async Task<List<Character>> GetOpponentCharacters() {
            return await _context.Characters.Where(c => c.User.Id != GetUserId()).ToListAsync();
        }
    }
}
