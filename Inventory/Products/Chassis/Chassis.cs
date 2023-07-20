using Models.Inventory.Configuration;

namespace Models.Inventory
{
    /**
     * Purpose: Chassis Class
     *          
     */       
    public class Chassis : Product
    {
        public Chassis() {}
        public Chassis(Item copy) : base(copy){}
        public Chassis(Model copy) : base(copy){}       
        // override the clone method inherited from item
        public override Item Clone() { return new Chassis(this); }     
    }
}