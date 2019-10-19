namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Implementation.Enums;

  public interface IStackFrameHandlerFactory
  {
    IStackFrameHandler Create(IStackFrameHandler parent, StackFrameScope stackFrameScopeType, ISeleniumScriptLogger seleniumScriptLogger);
  }
}
