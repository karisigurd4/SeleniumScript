namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Implementation.Enums;
  using global::SeleniumScript.Interfaces;
  using System.Collections.Generic;

  public class CallStack : ICallStack
  {
    private readonly ISeleniumScriptLogger seleniumScriptLogger;
    private readonly IStackFrameHandlerFactory stackFrameHandlerFactory;

    private readonly StackFrameHandler globalStackFrame;
    private readonly Stack<StackFrameHandler> stackFrames = new Stack<StackFrameHandler>();

    public IStackFrameHandler Current => stackFrames.Count > 0 ? stackFrames.Peek() : globalStackFrame;

    public CallStack(IStackFrameHandlerFactory stackFrameHandlerFactory, ISeleniumScriptLogger seleniumScriptLogger)
    {
      this.stackFrameHandlerFactory = stackFrameHandlerFactory;
      this.seleniumScriptLogger = seleniumScriptLogger;

      globalStackFrame = new StackFrameHandler(null, StackFrameScope.Global, seleniumScriptLogger);
    }

    public Function ResolveFunction(string name)
    {
      seleniumScriptLogger.Log($"Trying to resolve function {name}");
      return stackFrames.Count > 0 ? Current.ResolveFunction(name) : globalStackFrame.ResolveFunction(name);
    }

    public string ResolveVariable(string name)
    {
      seleniumScriptLogger.Log($"Trying to resolve variable {name}");
      return stackFrames.Count > 0 ? Current.ResolveVariable(name) ?? globalStackFrame.ResolveVariable(name) : globalStackFrame.ResolveVariable(name);
    }

    public void SetVariable(string name, string value)
    {
      seleniumScriptLogger.Log($"Trying to set variable {name} to value {value}");

      if (stackFrames.Count > 0 && Current.SetVariable(name, value))
        return;

      globalStackFrame.SetVariable(name, value);
    }

    public void Pop()
    {
      seleniumScriptLogger.Log($"Popping top of call stack");
      stackFrames.Pop();
    }

    public void Push(StackFrameScope stackFrameScopeType)
    {
      seleniumScriptLogger.Log($"Pushing scope with type {stackFrameScopeType} on top of call stack");
      stackFrames.Push(new StackFrameHandler(stackFrames.Count > 0 ? Current : globalStackFrame, stackFrameScopeType, seleniumScriptLogger));
    }
  }
}
