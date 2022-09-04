using Dappery.Domain.Dtos;

namespace Dappery.Domain.ViewModels;

public class ErrorViewModel
{
    public ErrorViewModel(ErrorDto errors) => Errors = errors;

    public ErrorDto Errors { get; }
}
