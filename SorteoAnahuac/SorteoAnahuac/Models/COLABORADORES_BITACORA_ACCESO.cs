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
    
    public partial class COLABORADORES_BITACORA_ACCESO
    {
        public long PK1 { get; set; }
        public long PK_COLABORADOR { get; set; }
        public string IP { get; set; }
        public System.DateTime FECHA_R { get; set; }
    
        public virtual COLABORADORE COLABORADORE { get; set; }
    }
}