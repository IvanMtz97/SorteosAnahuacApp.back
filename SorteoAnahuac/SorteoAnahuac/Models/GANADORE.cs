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
    
    public partial class GANADORE
    {
        public long PK1 { get; set; }
        public long PK_SORTEO { get; set; }
        public long PK_PREMIO { get; set; }
        public long PK_COMPRADOR { get; set; }
        public System.DateTime FECHA_R { get; set; }
        public string USUARIO { get; set; }
    
        public virtual SORTEO SORTEO { get; set; }
        public virtual PREMIO PREMIO { get; set; }
        public virtual COMPRADORE COMPRADOR { get; set; }
    }
}
