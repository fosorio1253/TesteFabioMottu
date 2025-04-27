namespace Vrumm.Application.Common.Interfaces;
public interface ICommand<out TResult> { }
public interface ICommand : ICommand<Unit> { }