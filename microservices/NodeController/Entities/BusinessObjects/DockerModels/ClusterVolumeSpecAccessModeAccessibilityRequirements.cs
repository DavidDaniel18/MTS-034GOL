/* 
 * Docker Engine API
 *
 * The Engine API is an HTTP API served by Docker Engine. It is the API the Docker client uses to communicate with the Engine, so everything the Docker client can do can be done with the API.  Most of the client's commands map directly to API endpoints (e.g. `docker ps` is `GET /containers/json`). The notable exception is running containers, which consists of several API calls.  # Errors  The API uses standard HTTP status codes to indicate the success or failure of the API call. The body of the response will be JSON in the following format:  ``` {   \"message\": \"page not found\" } ```  # Versioning  The API is usually changed in each release, so API calls are versioned to ensure that clients don't break. To lock to a specific version of the API, you prefix the URL with its version, for example, call `/v1.30/info` to use the v1.30 version of the `/info` endpoint. If the API version specified in the URL is not supported by the daemon, a HTTP `400 Bad Request` error message is returned.  If you omit the version-prefix, the current version of the API (v1.42) is used. For example, calling `/info` is the same as calling `/v1.42/info`. Using the API without a version-prefix is deprecated and will be removed in a future release.  Engine releases in the near future should support this version of the API, so your client will continue to work even if it is talking to a newer Engine.  The API uses an open schema model, which means server may add extra properties to responses. Likewise, the server will ignore any extra query parameters and request body properties. When you write clients, you need to ignore additional properties in responses to ensure they do not break when talking to newer daemons.   # Authentication  Authentication for registries is handled client side. The client has to send authentication details to various endpoints that need to communicate with registries, such as `POST /images/(name)/push`. These are sent as `X-Registry-Auth` header as a [base64url encoded](https://tools.ietf.org/html/rfc4648#section-5) (JSON) string with the following structure:  ``` {   \"username\": \"string\",   \"password\": \"string\",   \"email\": \"string\",   \"serveraddress\": \"string\" } ```  The `serveraddress` is a domain/IP without a protocol. Throughout this structure, double quotes are required.  If you have already got an identity token from the [`/auth` endpoint](#operation/SystemAuth), you can just pass this instead of credentials:  ``` {   \"identitytoken\": \"9cbaf023786cd7...\" } ``` 
 *
 * OpenAPI spec version: 1.42
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IO.Swagger.Model
{
    /// <summary>
    /// Requirements for the accessible topology of the volume. These fields are optional. For an in-depth description of what these fields mean, see the CSI specification. 
    /// </summary>
    [DataContract]
    public partial class ClusterVolumeSpecAccessModeAccessibilityRequirements :  IEquatable<ClusterVolumeSpecAccessModeAccessibilityRequirements>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterVolumeSpecAccessModeAccessibilityRequirements" /> class.
        /// </summary>
        /// <param name="requisite">A list of required topologies, at least one of which the volume must be accessible from. .</param>
        /// <param name="preferred">A list of topologies that the volume should attempt to be provisioned in. .</param>
        public ClusterVolumeSpecAccessModeAccessibilityRequirements(List<Topology> requisite = default(List<Topology>), List<Topology> preferred = default(List<Topology>))
        {
            this.Requisite = requisite;
            this.Preferred = preferred;
        }
        
        /// <summary>
        /// A list of required topologies, at least one of which the volume must be accessible from. 
        /// </summary>
        /// <value>A list of required topologies, at least one of which the volume must be accessible from. </value>
        [DataMember(Name="Requisite", EmitDefaultValue=false)]
        public List<Topology> Requisite { get; set; }

        /// <summary>
        /// A list of topologies that the volume should attempt to be provisioned in. 
        /// </summary>
        /// <value>A list of topologies that the volume should attempt to be provisioned in. </value>
        [DataMember(Name="Preferred", EmitDefaultValue=false)]
        public List<Topology> Preferred { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ClusterVolumeSpecAccessModeAccessibilityRequirements {\n");
            sb.Append("  Requisite: ").Append(Requisite).Append("\n");
            sb.Append("  Preferred: ").Append(Preferred).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as ClusterVolumeSpecAccessModeAccessibilityRequirements);
        }

        /// <summary>
        /// Returns true if ClusterVolumeSpecAccessModeAccessibilityRequirements instances are equal
        /// </summary>
        /// <param name="input">Instance of ClusterVolumeSpecAccessModeAccessibilityRequirements to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ClusterVolumeSpecAccessModeAccessibilityRequirements input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Requisite == input.Requisite ||
                    this.Requisite != null &&
                    this.Requisite.SequenceEqual(input.Requisite)
                ) && 
                (
                    this.Preferred == input.Preferred ||
                    this.Preferred != null &&
                    this.Preferred.SequenceEqual(input.Preferred)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Requisite != null)
                    hashCode = hashCode * 59 + this.Requisite.GetHashCode();
                if (this.Preferred != null)
                    hashCode = hashCode * 59 + this.Preferred.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
