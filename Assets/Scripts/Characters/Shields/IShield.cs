namespace Characters.Shields {
    public interface IShield {
        
        Stats ShieldStats { get; set; }
        void Block();
    }
}