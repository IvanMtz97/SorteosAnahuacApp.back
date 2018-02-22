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
    
    public partial class NICHO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NICHO()
        {
            this.COLABORADORES = new HashSet<COLABORADORE>();
        }
    
        public long PK1 { get; set; }
        public string CLAVE { get; set; }
        public string NICHO1 { get; set; }
        public string DESCRIPCION { get; set; }
        public long PK_SORTEO { get; set; }
        public long PK_SECTOR { get; set; }
        public long PK_USUARIO { get; set; }
        public System.DateTime FECHA_R { get; set; }
        public Nullable<System.DateTime> FECHA_M { get; set; }
        public bool ELIMINADO { get; set; }
        public Nullable<System.DateTime> LIMITE_VENTA { get; set; }
        public Nullable<System.DateTime> LIMITE_DEPOSITO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COLABORADORE> COLABORADORES { get; set; }
    }
}
