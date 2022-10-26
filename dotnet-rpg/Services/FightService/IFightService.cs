using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Utilities;

namespace dotnet_rpg.Services.FightService {
    public interface IFightService {

        Task<ServiceResponse<AttackResultDto>> WeaponAttackRandomOpponent(WeaponAttackRandomDto request);
        Task<ServiceResponse<AttackResultDto>> SkillAttackRandomOpponent(SkillAttackRandomDto request);
        Task<ServiceResponse<AttackResultDto>> WeaponAttackOpponent(WeaponAttackDto request);
        Task<ServiceResponse<AttackResultDto>> SkillAttackOpponent(SkillAttackDto request);
        Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request);
        Task<ServiceResponse<List<HightscoreDto>>> GetHightscore();
    }

}
