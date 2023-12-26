
   
    namespace Agric.Models
    {
        using System;
        using System.Collections.Generic;
        using System.ComponentModel.DataAnnotations;

        public partial class Devis
        {
            public System.Guid id { get; set; }
            public System.Guid id_client { get; set; }
            public System.DateTime date_demande { get; set; }
            public Nullable<bool> DemandeDevis { get; set; }
            public Nullable<bool> DevisDelete { get; set; }
            public Nullable<bool> DevisAccepter { get; set; }
            public int NumDevis { get; set; }
            [Display(Name = "Devis")]
            public string Devis1 { get; set; }
            public Nullable<bool> DevieEnvoyer { get; set; }

            public virtual Users Users { get; set; }
        }
    }
