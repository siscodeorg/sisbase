using DSharpPlus.Interactivity;

namespace LA_RPbot.Discord.Utils
{
    /// <summary>
    /// Interface for <see cref="IApplicableSystem"/> that can be applied to <see cref="InteractivityExtension"/>
    /// </summary>
    public interface IApplyToInteractivity
    {
        void ApplyToInteractivity(InteractivityExtension interactivity);
    }
}