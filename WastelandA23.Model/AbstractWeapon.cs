//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WastelandA23.Model
{
    using System;
    using System.Collections.Generic;
    
    public abstract partial class AbstractWeapon : Item
    {
        public AbstractWeapon()
        {
            this.AbstractWeaponItems = new HashSet<AbstractWeaponItem>();
        }
    
    
        public virtual Magazine Magazine { get; set; }
        public virtual ICollection<AbstractWeaponItem> AbstractWeaponItems { get; set; }
    }
}