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
    
    public partial class ESTADO_CUENTA
    {
        public long PK_SORTEO { get; set; }
        public long PK_COLABORADOR { get; set; }
        public long PK_SECTOR { get; set; }
        public long PK_NICHO { get; set; }
        public Nullable<decimal> MONTO { get; set; }
        public Nullable<decimal> COMISION { get; set; }
        public Nullable<decimal> IMPORTE { get; set; }
        public decimal ABONO { get; set; }
        public Nullable<decimal> SALDO { get; set; }
    }
}