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
    
    public partial class BOLETO_TAB
    {
        public int PK1 { get; set; }
        public string FOLIO { get; set; }
        public Nullable<decimal> COSTO { get; set; }
        public int SORTEO { get; set; }
        public string FORMATO { get; set; }
        public string USUARIO { get; set; }
        public Nullable<System.DateTime> FECHA_R { get; set; }
        public Nullable<System.DateTime> FECHA_M { get; set; }
        public bool ASIGNADO { get; set; }
        public string VENDIDO { get; set; }
        public Nullable<decimal> ABONO { get; set; }
        public long PK_TALONARIO { get; set; }
        public Nullable<bool> RETORNADO { get; set; }
        public string INCIDENCIA { get; set; }
        public string TALONARIO { get; set; }
        public Nullable<int> FOLIODIGITAL { get; set; }
    }
}
