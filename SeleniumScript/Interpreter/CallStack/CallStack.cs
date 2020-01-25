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
      var function = stackFrames.Count > 0 ? Current.ResolveFunction(name) : globalStackFrame.ResolveFunction(name);
      if (function == null) 
      {
        seleniumScriptLogger.Log($"Function {name} could not be resolved", SeleniumScriptLogLevel.InterpreterDetails);
        throw new SeleniumScriptVisitorException($"Function {name} could not be resolved");
      }
      return function;
    }

    public Variable ResolveVariable(string name)
    {
      seleniumScriptLogger.Log($"Trying to resolve variable {name}");
      var variable = stackFrames.Count > 0 ? Current.ResolveVariable(name) ?? globalStackFrame.ResolveVariable(name) : globalStackFrame.ResolveVariable(name);
      if (variable == null) 
      {
        seleniumScriptLogger.Log($"Varriable {name} could not be resolved", SeleniumScriptLogLevel.InterpreterDetails);
        throw new SeleniumScriptVisitorException($"Varriable {name} could not be resolved");
      }
      return variable;
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
