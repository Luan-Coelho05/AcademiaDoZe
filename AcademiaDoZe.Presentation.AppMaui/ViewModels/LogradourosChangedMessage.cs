
// Luan Coelho

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public class LogradourosChangedMessage : ValueChangedMessage<bool>
{
    public LogradourosChangedMessage(bool value = true) : base(value)
    {
    }
}