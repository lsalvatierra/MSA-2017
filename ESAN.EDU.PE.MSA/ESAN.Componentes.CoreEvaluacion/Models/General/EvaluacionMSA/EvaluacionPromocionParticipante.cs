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
    
    public partial class EvaluacionPromocionParticipante
    {
        public int EvaluacionPromocionID { get; set; }
        public long ParticipanteID { get; set; }
        public int EvaluacionMedicionID { get; set; }
        public Nullable<bool> EsExterno { get; set; }
    
        public virtual EvaluacionMedicion EvaluacionMedicion { get; set; }
        public virtual EvaluacionPromocion EvaluacionPromocion { get; set; }
        public virtual Participante Participante { get; set; }
    }
}