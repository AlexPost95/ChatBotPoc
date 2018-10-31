// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Bot.Connector
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Conversation Transcript
    /// </summary>
    public partial class Transcript
    {
        /// <summary>
        /// Initializes a new instance of the Transcript class.
        /// </summary>
        public Transcript()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Transcript class.
        /// </summary>
        /// <param name="activities">Array of conversation activities.</param>
        public Transcript(IList<Activity> activities = default(IList<Activity>))
        {
            Activities = activities;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets array of conversation activities.
        /// </summary>
        [JsonProperty(PropertyName = "activities")]
        public IList<Activity> Activities { get; set; }

    }
}
