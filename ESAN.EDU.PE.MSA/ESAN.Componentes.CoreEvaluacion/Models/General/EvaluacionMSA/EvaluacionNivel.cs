//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ESAN.Componentes.CoreEvaluacion.Models.General.EvaluacionMSA
{
    using System;
    using System.Collections.Generic;
    
    public partial class EvaluacionNivel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EvaluacionNivel()
        {
            this.EvaluacionNivel1 = new HashSet<EvaluacionNivel>();
            this.EvaluacionPregunta = new HashSet<EvaluacionPregunta>();
        }
    
        public long EvaluacionNivelID { get; set; }
        public Nullable<int> EvaluacionID { get; set; }
        public string EvaluacionNivelDescripcion { get; set; }
        public Nullable<long> EvaluacionNivelPadreID { get; set; }
        public Nullable<int> EvaluacionNivelOrden { get; set; }
        public Nullable<int> EvaluacionNivelTipoCalificacion { get; set; }
        public Nullable<bool> EvaluacionNivelEstado { get; set; }
    
        public virtual Evaluacion Evaluacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EvaluacionNivel> EvaluacionNivel1 { get; set; }
        public virtual EvaluacionNivel EvaluacionNivel2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EvaluacionPregunta> EvaluacionPregunta { get; set; }
    }
}