using contactPro2.Models.Enums;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contactPro2.Models
{
    public class Contact  // everything below available to this class
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;
        private DateTimeOffset? _dateOfBirth;


        // Primary Key
        public int Id { get; set; }

        // Foreign Key
        [Required]
        public string? AppUserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        [NotMapped]
        public string? FullName2 { get { return $"{LastName} {FirstName}"; } }

        public DateTimeOffset Created { get { return _created; } set { _created = value.ToUniversalTime(); } }

        public DateTimeOffset? Updated
        {
            get => _updated;
            set
            {
                if (value.HasValue)
                {
                    _updated = value.Value.ToUniversalTime();
                }
            }
        }

        [Display(Name ="Birth Date")]

        public DateTimeOffset? DateOfBirth { get => _dateOfBirth;
            set
            {
                if (value.HasValue)
                {
                    _dateOfBirth = value.Value.ToUniversalTime();
                }

            }

        }


        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public States? State { get; set; }

        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        public string? ZipCode { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }


        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        // Navigation Properties
        public virtual AppUser? AppUser { get; set; }

        // Relation to Category

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();


    }

}
