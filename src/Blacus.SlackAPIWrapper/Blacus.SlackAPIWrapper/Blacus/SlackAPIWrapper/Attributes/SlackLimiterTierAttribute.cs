using System;
using System.Linq;
using System.Reflection;

#pragma warning disable CS8603 // Possible null reference return.

namespace Blacus.SlackAPIWrapper.Attributes
{

    public class SlackLimiterTierAttribute : Attribute
    {

        public LimiterTier LimiterTier { get; }

        public SlackLimiterTierAttribute(LimiterTier limiterTier)
        {
            LimiterTier = limiterTier;
        }

        public static SlackLimiterTierAttribute GetSlackLimiterAttribute<T>()
        {

            var type = typeof(T);
            TypeInfo info = type.GetTypeInfo();
            var slackMethodAttribute = info.GetCustomAttributes<SlackLimiterTierAttribute>().FirstOrDefault();

            return slackMethodAttribute;

        }

    }

}