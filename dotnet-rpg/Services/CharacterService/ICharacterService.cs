using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Utilities;

namespace dotnet_rpg.Services.CharacterService {
    public interface ICharacterService {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllUserCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id);
        Task<Character> GetCharacterById(int id);
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter);
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
        Task<ServiceResponse<List<GetCharacterDto>>> ResetHP();


 }
}
