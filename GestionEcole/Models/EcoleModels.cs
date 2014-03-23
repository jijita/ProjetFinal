using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GestionEcole.Models
{
    public abstract class Personne
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredNom", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        [Display(Name = "DisplayNom", ResourceType = typeof(Resources.Models.Personne))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthNom", ErrorMessageResourceType = typeof(Resources.Models.Personne), MinimumLength = 2)]
        public string Nom { get; set; }

        [Required(ErrorMessageResourceName = "RequiredPrenom", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        [Display(Name = "DisplayPrenom", ResourceType = typeof(Resources.Models.Personne))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthPrenom", ErrorMessageResourceType = typeof(Resources.Models.Personne), MinimumLength = 2)]
        public string Prenom { get; set; }

        [Required(ErrorMessageResourceName = "RequiredEmail", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        [Display(Name = "DisplayEmail", ResourceType = typeof(Resources.Models.Personne))]
        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        				            [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        				            [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessageResourceName = "RegExEmail", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredDateNaissance", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        [Display(Name = "DisplayDateNaissance", ResourceType = typeof(Resources.Models.Personne))]
        [DataType(DataType.Date)]
        public System.DateTime DateNaissance { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAdresse", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        [Display(Name = "DisplayAdresse", ResourceType = typeof(Resources.Models.Personne))]
        [DataType(DataType.MultilineText)]
        [StringLength(100, ErrorMessageResourceName = "StringLengthAdresse", ErrorMessageResourceType = typeof(Resources.Models.Personne))]
        public string Adresse { get; set; }

        [Display(Name = "DisplayPhoto", ResourceType = typeof(Resources.Models.Personne))]
        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }
    }

    public class Enseignant : Personne
    {
        public Enseignant()
            : base()
        {
            this.Evaluations = new HashSet<Evaluation>();
            this.Cours = new HashSet<Cours>();
        }

        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Cours> Cours { get; set; }
    }

    #region Region Etudiant et ses cours

    public class Etudiant : Personne
    {
        public Etudiant()
            : base()
        {
            this.PresenceCours = new HashSet<PresenceCours>();
            this.Evaluations = new HashSet<Evaluation>();
        }

        [NotMapped]
        [Display(Name = "DisplayMoyenne", ResourceType = typeof(Resources.Models.Etudiant))]
        [Range(0, 100, ErrorMessageResourceName="RangeMoyenne", ErrorMessageResourceType = typeof(Resources.Models.Etudiant))]
        public decimal? Moyenne { get; set; }

        public virtual ICollection<PresenceCours> PresenceCours { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
    }

    public class PresenceCours
    {
        [Key, Column(Order = 0)]
        public int EtudiantId { get; set; }

        [Key, Column(Order = 1)]
        public int CoursId { get; set; }

        [Display(Name = "DisplayMotif", ResourceType = typeof(Resources.Models.PresenceCours))]
        [DataType(DataType.MultilineText)]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMotif", ErrorMessageResourceType = typeof(Resources.Models.PresenceCours))]
        public string Motif { get; set; }

        [Display(Name = "DispalyAbscence", ResourceType = typeof(Resources.Models.PresenceCours))]
        public bool Absence { get; set; }

        public virtual Etudiant Etudiant { get; set; }
        public virtual Cours Cours { get; set; }
    }

    public class Cours
    {
        public Cours()
        {
            this.PresenceCours = new HashSet<PresenceCours>();
        }

        [Key]
        public int CoursId { get; set; }
        
        public int MatiereId { get; set; }
        public int EnseignantId { get; set; }

        [Required]
        [Display(Name = "DisplayDateDebut", ResourceType = typeof(Resources.Models.Cours))]
        [DataType(DataType.DateTime)]
        public System.DateTime DateDebut { get; set; }

        [Required]
        [Display(Name = "DisplayDateFin", ResourceType = typeof(Resources.Models.Cours))]
        [DataType(DataType.DateTime)]
        public System.DateTime DateFin { get; set; }

        [Display(Name = "DisplayDescription", ResourceType = typeof(Resources.Models.Cours))]
        [DataType(DataType.MultilineText)]
        [StringLength(100, ErrorMessageResourceName = "StringLengthDescription", ErrorMessageResourceType = typeof(Resources.Models.Cours))]
        public string Description { get; set; }

        public virtual ICollection<PresenceCours> PresenceCours { get; set; }

        [ForeignKey("MatiereId")]
        public virtual Matiere Matiere { get; set; }
        [ForeignKey("EnseignantId")]
        public virtual Enseignant Enseignant { get; set; }
    }

    #endregion

    public class Matiere
    {
        public Matiere()
        {
            this.Cours = new HashSet<Cours>();
            this.Evaluations = new HashSet<Evaluation>();
        }

        [Key]
        public int MatiereId { get; set; }

        [Required]
        [Display(Name = "DisplayTitre", ResourceType = typeof(Resources.Models.Matiere))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthTitre", ErrorMessageResourceType = typeof(Resources.Models.Matiere), MinimumLength = 1)]
        public string Titre { get; set; }

        [Display(Name = "DisplayDescription", ResourceType = typeof(Resources.Models.Matiere))]
        [DataType(DataType.MultilineText)]
        [StringLength(100, ErrorMessageResourceName = "StringLengthDescription", ErrorMessageResourceType = typeof(Resources.Models.Matiere))]
        public string Description { get; set; }

        public virtual ICollection<Cours> Cours { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        public class Comparer : IEqualityComparer<Matiere>
        {
            public bool Equals(Matiere x, Matiere y)
            {
                return x.MatiereId == y.MatiereId
                    && x.Titre.Equals(y.Titre)
                    && x.Description.Equals(y.Description);
            }

            public int GetHashCode(Matiere a)
            {
                return a.MatiereId.GetHashCode() + a.Titre.GetHashCode() + a.Description.GetHashCode(); ;
            }
        }
    }

    #region Region Evaluation

    public class Evaluation
    {
        [Key, Column(Order = 0)]
        public int EtudiantId { get; set; }

        [Key, Column(Order = 1)]
        public int EnseignantId { get; set; }

        [Key, Column(Order = 2)]
        public int MatiereId { get; set; }

        [Key, Column(Order = 3)]
        public int TypeEvaluationId { get; set; }

        [Required]
        [Display(Name = "DisplayNote", ResourceType = typeof(Resources.Models.Evaluation))]
        [Range(0, 100, ErrorMessageResourceName = "RangeNote", ErrorMessageResourceType = typeof(Resources.Models.Evaluation))]
        public decimal Note { get; set; }

        public virtual TypeEvaluation TypeEvaluation { get; set; }
        public virtual Matiere Matiere { get; set; }
        public virtual Enseignant Enseignant { get; set; }
        public virtual Etudiant Etudiant { get; set; }
    }

    public class TypeEvaluation
    {
        public TypeEvaluation()
        {
            this.Evaluations = new HashSet<Evaluation>();
        }

        [Key]
        public int TypeEvaluationId { get; set; }

        [Display(Name = "DisplayType", ResourceType = typeof(Resources.Models.TypeEvaluation))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthType", ErrorMessageResourceType = typeof(Resources.Models.TypeEvaluation), MinimumLength = 2)]
        public string Type { get; set; }

        [Required]
        [Display(Name = "DisplayPonderation", ResourceType = typeof(Resources.Models.TypeEvaluation))]
        [Range(0, 100, ErrorMessageResourceName = "RangePonderation", ErrorMessageResourceType = typeof(Resources.Models.TypeEvaluation))]
        public decimal Ponderation { get; set; }

        public virtual ICollection<Evaluation> Evaluations { get; set; }
    }

    #endregion

    public partial class EcoleContainer : DbContext
    {
        public EcoleContainer()
            : base("name=EcoleContainer")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EcoleContainer>());
        }

        public virtual DbSet<Enseignant> Enseignants { get; set; }
        public virtual DbSet<Etudiant> Etudiants { get; set; }
        public virtual DbSet<PresenceCours> PresenceCours { get; set; }
        public virtual DbSet<Cours> Cours { get; set; }
        public virtual DbSet<Matiere> Matieres { get; set; }
        public virtual DbSet<TypeEvaluation> TypeEvaluations { get; set; }
        public virtual DbSet<Evaluation> Evaluations { get; set; }
    }
}