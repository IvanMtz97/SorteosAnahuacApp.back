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
    
    public partial class SORTEOS_SECTORES_BOLETOS
    {
        public long PK1 { get; set; }
        public long PK_BOLETO { get; set; }
        public long PK_TALONARIO { get; set; }
        public long PK_SORTEO { get; set; }
        public long PK_SECTOR { get; set; }
        public string FORMATO { get; set; }
        public string USUARIO { get; set; }
        public Nullable<System.DateTime> FECHA_R { get; set; }
        public Nullable<System.DateTime> FECHA_M { get; set; }
        public Nullable<bool> ASIGNADO { get; set; }
    }
}
