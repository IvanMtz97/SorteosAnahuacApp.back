//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SorteoAnahuac.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ROLES_USUARIO
    {
        public long PK1 { get; set; }
        public long PK_USUARIO { get; set; }
        public long PK_ROLE { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
        public virtual ROLE ROL { get; set; }
    }
}