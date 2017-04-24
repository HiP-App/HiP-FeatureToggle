// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Clients.Models
{
    using System.Linq;

    public partial class UsersFormModel
    {
        /// <summary>
        /// Initializes a new instance of the UsersFormModel class.
        /// </summary>
        public UsersFormModel() { }

        /// <summary>
        /// Initializes a new instance of the UsersFormModel class.
        /// </summary>
        public UsersFormModel(System.Collections.Generic.IList<string> users)
        {
            Users = users;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "users")]
        public System.Collections.Generic.IList<string> Users { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Users == null)
            {
                throw new Microsoft.Rest.ValidationException(Microsoft.Rest.ValidationRules.CannotBeNull, "Users");
            }
        }
    }
}