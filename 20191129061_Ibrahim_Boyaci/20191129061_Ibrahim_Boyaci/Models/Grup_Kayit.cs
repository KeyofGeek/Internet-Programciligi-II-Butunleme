//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _20191129061_Ibrahim_Boyaci.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Grup_Kayit
    {
        public string kayitId { get; set; }
        public string kayGrupId { get; set; }
        public string kayUyeId { get; set; }
    
        public virtual Gruplar Gruplar { get; set; }
        public virtual Uyeler Uyeler { get; set; }
    }
}
