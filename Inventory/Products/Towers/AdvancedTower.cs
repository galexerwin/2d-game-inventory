namespace Models.Inventory
{
    /**
     * Purpose: Advanced Tower Class
     *          Utilizes Advanced Skills
     */      
    public class AdvancedTower : Tower
    {
        public AdvancedSkills AdvancedSkill { get; set; }
        public int SkillCoolDown { get; set; }
    }
}