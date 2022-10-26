namespace dotnet_rpg.Models {
    public class Character {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int HitPoints { get; set; } = 100;
        public int Strengh { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.C;
        public Weapon Weapon { get; set; }
        public List<Skill> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
        public User? User { get; set; }

    }
}
