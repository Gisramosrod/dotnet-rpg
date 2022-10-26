namespace dotnet_rpg.Dtos.Character {
    public class AddCharacterDto {
        public string Name { get; set; } = String.Empty;
        public int HitPoints { get; set; } = 100;
        public int Strengh { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.C;
    }
}
