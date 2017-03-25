using System.ComponentModel.DataAnnotations;

namespace proApp.Models
{
    public class LoggingModel: PocoBase
    {
        [Display(Name = "Name")]
        [MaxLength(32), MinLength(4)]
        public string UserName
        {
            get {  return GetValue <string>(); }
            set {  SetValue( value ); }
        }

        [Required]
        public string Password {
            get { return GetValue<string>(); }
            set { SetValue( value ); }
        }
    }

}
