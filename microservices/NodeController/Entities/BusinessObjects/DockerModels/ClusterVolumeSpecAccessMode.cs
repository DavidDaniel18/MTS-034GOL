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
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace IO.Swagger.Model
{
    /// <summary>
    /// Defines how the volume is used by tasks. 
    /// </summary>
    [DataContract]
    public partial class ClusterVolumeSpecAccessMode :  IEquatable<ClusterVolumeSpecAccessMode>, IValidatableObject
    {
        /// <summary>
        /// The set of nodes this volume can be used on at one time. - &#x60;single&#x60; The volume may only be scheduled to one node at a time. - &#x60;multi&#x60; the volume may be scheduled to any supported number of nodes at a time. 
        /// </summary>
        /// <value>The set of nodes this volume can be used on at one time. - &#x60;single&#x60; The volume may only be scheduled to one node at a time. - &#x60;multi&#x60; the volume may be scheduled to any supported number of nodes at a time. </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ScopeEnum
        {
            
            /// <summary>
            /// Enum Single for value: single
            /// </summary>
            [EnumMember(Value = "single")]
            Single = 1,
            
            /// <summary>
            /// Enum Multi for value: multi
            /// </summary>
            [EnumMember(Value = "multi")]
            Multi = 2
        }

        /// <summary>
        /// The set of nodes this volume can be used on at one time. - &#x60;single&#x60; The volume may only be scheduled to one node at a time. - &#x60;multi&#x60; the volume may be scheduled to any supported number of nodes at a time. 
        /// </summary>
        /// <value>The set of nodes this volume can be used on at one time. - &#x60;single&#x60; The volume may only be scheduled to one node at a time. - &#x60;multi&#x60; the volume may be scheduled to any supported number of nodes at a time. </value>
        [DataMember(Name="Scope", EmitDefaultValue=false)]
        public ScopeEnum? Scope { get; set; }
        /// <summary>
        /// The number and way that different tasks can use this volume at one time. - &#x60;none&#x60; The volume may only be used by one task at a time. - &#x60;readonly&#x60; The volume may be used by any number of tasks, but they all must mount the volume as readonly - &#x60;onewriter&#x60; The volume may be used by any number of tasks, but only one may mount it as read/write. - &#x60;all&#x60; The volume may have any number of readers and writers. 
        /// </summary>
        /// <value>The number and way that different tasks can use this volume at one time. - &#x60;none&#x60; The volume may only be used by one task at a time. - &#x60;readonly&#x60; The volume may be used by any number of tasks, but they all must mount the volume as readonly - &#x60;onewriter&#x60; The volume may be used by any number of tasks, but only one may mount it as read/write. - &#x60;all&#x60; The volume may have any number of readers and writers. </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SharingEnum
        {
            
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            
            /// <summary>
            /// Enum Readonly for value: readonly
            /// </summary>
            [EnumMember(Value = "readonly")]
            Readonly = 2,
            
            /// <summary>
            /// Enum Onewriter for value: onewriter
            /// </summary>
            [EnumMember(Value = "onewriter")]
            Onewriter = 3,
            
            /// <summary>
            /// Enum All for value: all
            /// </summary>
            [EnumMember(Value = "all")]
            All = 4
        }

        /// <summary>
        /// The number and way that different tasks can use this volume at one time. - &#x60;none&#x60; The volume may only be used by one task at a time. - &#x60;readonly&#x60; The volume may be used by any number of tasks, but they all must mount the volume as readonly - &#x60;onewriter&#x60; The volume may be used by any number of tasks, but only one may mount it as read/write. - &#x60;all&#x60; The volume may have any number of readers and writers. 
        /// </summary>
        /// <value>The number and way that different tasks can use this volume at one time. - &#x60;none&#x60; The volume may only be used by one task at a time. - &#x60;readonly&#x60; The volume may be used by any number of tasks, but they all must mount the volume as readonly - &#x60;onewriter&#x60; The volume may be used by any number of tasks, but only one may mount it as read/write. - &#x60;all&#x60; The volume may have any number of readers and writers. </value>
        [DataMember(Name="Sharing", EmitDefaultValue=false)]
        public SharingEnum? Sharing { get; set; }
        /// <summary>
        /// The availability of the volume for use in tasks. - &#x60;active&#x60; The volume is fully available for scheduling on the cluster - &#x60;pause&#x60; No new workloads should use the volume, but existing workloads are not stopped. - &#x60;drain&#x60; All workloads using this volume should be stopped and rescheduled, and no new ones should be started. 
        /// </summary>
        /// <value>The availability of the volume for use in tasks. - &#x60;active&#x60; The volume is fully available for scheduling on the cluster - &#x60;pause&#x60; No new workloads should use the volume, but existing workloads are not stopped. - &#x60;drain&#x60; All workloads using this volume should be stopped and rescheduled, and no new ones should be started. </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AvailabilityEnum
        {
            
            /// <summary>
            /// Enum Active for value: active
            /// </summary>
            [EnumMember(Value = "active")]
            Active = 1,
            
            /// <summary>
            /// Enum Pause for value: pause
            /// </summary>
            [EnumMember(Value = "pause")]
            Pause = 2,
            
            /// <summary>
            /// Enum Drain for value: drain
            /// </summary>
            [EnumMember(Value = "drain")]
            Drain = 3
        }

        /// <summary>
        /// The availability of the volume for use in tasks. - &#x60;active&#x60; The volume is fully available for scheduling on the cluster - &#x60;pause&#x60; No new workloads should use the volume, but existing workloads are not stopped. - &#x60;drain&#x60; All workloads using this volume should be stopped and rescheduled, and no new ones should be started. 
        /// </summary>
        /// <value>The availability of the volume for use in tasks. - &#x60;active&#x60; The volume is fully available for scheduling on the cluster - &#x60;pause&#x60; No new workloads should use the volume, but existing workloads are not stopped. - &#x60;drain&#x60; All workloads using this volume should be stopped and rescheduled, and no new ones should be started. </value>
        [DataMember(Name="Availability", EmitDefaultValue=false)]
        public AvailabilityEnum? Availability { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterVolumeSpecAccessMode" /> class.
        /// </summary>
        /// <param name="scope">The set of nodes this volume can be used on at one time. - &#x60;single&#x60; The volume may only be scheduled to one node at a time. - &#x60;multi&#x60; the volume may be scheduled to any supported number of nodes at a time.  (default to ScopeEnum.Single).</param>
        /// <param name="sharing">The number and way that different tasks can use this volume at one time. - &#x60;none&#x60; The volume may only be used by one task at a time. - &#x60;readonly&#x60; The volume may be used by any number of tasks, but they all must mount the volume as readonly - &#x60;onewriter&#x60; The volume may be used by any number of tasks, but only one may mount it as read/write. - &#x60;all&#x60; The volume may have any number of readers and writers.  (default to SharingEnum.None).</param>
        /// <param name="mountVolume">Options for using this volume as a Mount-type volume.      Either MountVolume or BlockVolume, but not both, must be     present.   properties:     FsType:       type: \&quot;string\&quot;       description: |         Specifies the filesystem type for the mount volume.         Optional.     MountFlags:       type: \&quot;array\&quot;       description: |         Flags to pass when mounting the volume. Optional.       items:         type: \&quot;string\&quot; BlockVolume:   type: \&quot;object\&quot;   description: |     Options for using this volume as a Block-type volume.     Intentionally empty. .</param>
        /// <param name="secrets">Swarm Secrets that are passed to the CSI storage plugin when operating on this volume. .</param>
        /// <param name="accessibilityRequirements">accessibilityRequirements.</param>
        /// <param name="capacityRange">capacityRange.</param>
        /// <param name="availability">The availability of the volume for use in tasks. - &#x60;active&#x60; The volume is fully available for scheduling on the cluster - &#x60;pause&#x60; No new workloads should use the volume, but existing workloads are not stopped. - &#x60;drain&#x60; All workloads using this volume should be stopped and rescheduled, and no new ones should be started.  (default to AvailabilityEnum.Active).</param>
        public ClusterVolumeSpecAccessMode(ScopeEnum? scope = ScopeEnum.Single, SharingEnum? sharing = SharingEnum.None, Object mountVolume = default(Object), List<ClusterVolumeSpecAccessModeSecrets> secrets = default(List<ClusterVolumeSpecAccessModeSecrets>), ClusterVolumeSpecAccessModeAccessibilityRequirements accessibilityRequirements = default(ClusterVolumeSpecAccessModeAccessibilityRequirements), ClusterVolumeSpecAccessModeCapacityRange capacityRange = default(ClusterVolumeSpecAccessModeCapacityRange), AvailabilityEnum? availability = AvailabilityEnum.Active)
        {
            // use default value if no "scope" provided
            if (scope == null)
            {
                this.Scope = ScopeEnum.Single;
            }
            else
            {
                this.Scope = scope;
            }
            // use default value if no "sharing" provided
            if (sharing == null)
            {
                this.Sharing = SharingEnum.None;
            }
            else
            {
                this.Sharing = sharing;
            }
            this.MountVolume = mountVolume;
            this.Secrets = secrets;
            this.AccessibilityRequirements = accessibilityRequirements;
            this.CapacityRange = capacityRange;
            // use default value if no "availability" provided
            if (availability == null)
            {
                this.Availability = AvailabilityEnum.Active;
            }
            else
            {
                this.Availability = availability;
            }
        }
        


        /// <summary>
        /// Options for using this volume as a Mount-type volume.      Either MountVolume or BlockVolume, but not both, must be     present.   properties:     FsType:       type: \&quot;string\&quot;       description: |         Specifies the filesystem type for the mount volume.         Optional.     MountFlags:       type: \&quot;array\&quot;       description: |         Flags to pass when mounting the volume. Optional.       items:         type: \&quot;string\&quot; BlockVolume:   type: \&quot;object\&quot;   description: |     Options for using this volume as a Block-type volume.     Intentionally empty. 
        /// </summary>
        /// <value>Options for using this volume as a Mount-type volume.      Either MountVolume or BlockVolume, but not both, must be     present.   properties:     FsType:       type: \&quot;string\&quot;       description: |         Specifies the filesystem type for the mount volume.         Optional.     MountFlags:       type: \&quot;array\&quot;       description: |         Flags to pass when mounting the volume. Optional.       items:         type: \&quot;string\&quot; BlockVolume:   type: \&quot;object\&quot;   description: |     Options for using this volume as a Block-type volume.     Intentionally empty. </value>
        [DataMember(Name="MountVolume", EmitDefaultValue=false)]
        public Object MountVolume { get; set; }

        /// <summary>
        /// Swarm Secrets that are passed to the CSI storage plugin when operating on this volume. 
        /// </summary>
        /// <value>Swarm Secrets that are passed to the CSI storage plugin when operating on this volume. </value>
        [DataMember(Name="Secrets", EmitDefaultValue=false)]
        public List<ClusterVolumeSpecAccessModeSecrets> Secrets { get; set; }

        /// <summary>
        /// Gets or Sets AccessibilityRequirements
        /// </summary>
        [DataMember(Name="AccessibilityRequirements", EmitDefaultValue=false)]
        public ClusterVolumeSpecAccessModeAccessibilityRequirements AccessibilityRequirements { get; set; }

        /// <summary>
        /// Gets or Sets CapacityRange
        /// </summary>
        [DataMember(Name="CapacityRange", EmitDefaultValue=false)]
        public ClusterVolumeSpecAccessModeCapacityRange CapacityRange { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ClusterVolumeSpecAccessMode {\n");
            sb.Append("  Scope: ").Append(Scope).Append("\n");
            sb.Append("  Sharing: ").Append(Sharing).Append("\n");
            sb.Append("  MountVolume: ").Append(MountVolume).Append("\n");
            sb.Append("  Secrets: ").Append(Secrets).Append("\n");
            sb.Append("  AccessibilityRequirements: ").Append(AccessibilityRequirements).Append("\n");
            sb.Append("  CapacityRange: ").Append(CapacityRange).Append("\n");
            sb.Append("  Availability: ").Append(Availability).Append("\n");
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
            return this.Equals(input as ClusterVolumeSpecAccessMode);
        }

        /// <summary>
        /// Returns true if ClusterVolumeSpecAccessMode instances are equal
        /// </summary>
        /// <param name="input">Instance of ClusterVolumeSpecAccessMode to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ClusterVolumeSpecAccessMode input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Scope == input.Scope ||
                    (this.Scope != null &&
                    this.Scope.Equals(input.Scope))
                ) && 
                (
                    this.Sharing == input.Sharing ||
                    (this.Sharing != null &&
                    this.Sharing.Equals(input.Sharing))
                ) && 
                (
                    this.MountVolume == input.MountVolume ||
                    (this.MountVolume != null &&
                    this.MountVolume.Equals(input.MountVolume))
                ) && 
                (
                    this.Secrets == input.Secrets ||
                    this.Secrets != null &&
                    this.Secrets.SequenceEqual(input.Secrets)
                ) && 
                (
                    this.AccessibilityRequirements == input.AccessibilityRequirements ||
                    (this.AccessibilityRequirements != null &&
                    this.AccessibilityRequirements.Equals(input.AccessibilityRequirements))
                ) && 
                (
                    this.CapacityRange == input.CapacityRange ||
                    (this.CapacityRange != null &&
                    this.CapacityRange.Equals(input.CapacityRange))
                ) && 
                (
                    this.Availability == input.Availability ||
                    (this.Availability != null &&
                    this.Availability.Equals(input.Availability))
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
                if (this.Scope != null)
                    hashCode = hashCode * 59 + this.Scope.GetHashCode();
                if (this.Sharing != null)
                    hashCode = hashCode * 59 + this.Sharing.GetHashCode();
                if (this.MountVolume != null)
                    hashCode = hashCode * 59 + this.MountVolume.GetHashCode();
                if (this.Secrets != null)
                    hashCode = hashCode * 59 + this.Secrets.GetHashCode();
                if (this.AccessibilityRequirements != null)
                    hashCode = hashCode * 59 + this.AccessibilityRequirements.GetHashCode();
                if (this.CapacityRange != null)
                    hashCode = hashCode * 59 + this.CapacityRange.GetHashCode();
                if (this.Availability != null)
                    hashCode = hashCode * 59 + this.Availability.GetHashCode();
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
