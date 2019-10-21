namespace SeleniumScript.Factories
{
  using SeleniumScript.Implementation;
  using SeleniumScript.Implementation.Enums;
  using SeleniumScript.Interfaces;
  
  public class StackFrameHandlerFactory : IStackFrameHandlerFactory
  {
    public IStackFrameHandler Create(IStackFrameHandler parent, StackFrameScope stackFrameScopeType, ISeleniumScriptLogger seleniumScriptLogger)
    {
      return new StackFrameHandler(parent, stackFrameScopeType, seleniumScriptLogger);
    }
  }
}
