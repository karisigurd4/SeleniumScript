namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Implementation.Enums;
  using global::SeleniumScript.Interfaces;
  using System.Collections.Generic;

  public class StackFrameHandler : IStackFrameHandler
  {
    private readonly ISeleniumScriptLogger seleniumScriptLogger;

    private IStackFrameHandler parent { get; set; }
    private StackFrameScope scopeType { get; set; }

    private Dictionary<string, string> variables { get; } = new Dictionary<string, string>();
    private Dictionary<string, Function> functions { get; } = new Dictionary<string, Function>();

    public StackFrameHandler(IStackFrameHandler parent, StackFrameScope scopeType, ISeleniumScriptLogger seleniumScriptLogger)
    {
      this.seleniumScriptLogger = seleniumScriptLogger;
      this.parent = parent;
      this.scopeType = scopeType;
    }

    public void AddVariable(string name, string value = null)
    {
      seleniumScriptLogger.Log($"Adding variable {name} with value {value}", SeleniumScriptLogLevel.InterpreterDetails);
      if (variables.ContainsKey(name))
      {
        throw new SeleniumScriptVisitorException($"Redeclaration of {name} within current scope");
      }

      variables.Add(name, value);
    }

    public bool SetVariable(string name, string value) {
      seleniumScriptLogger.Log($"Setting value of variable {name} to {value}", SeleniumScriptLogLevel.InterpreterDetails);
      if (variables.ContainsKey(name))
      {
        variables[name] = value;
        return true;
      }

      if (scopeType == StackFrameScope.Method || scopeType == StackFrameScope.Global)
      {
        seleniumScriptLogger.Log($"Varriable {name} could not be resolved", SeleniumScriptLogLevel.InterpreterDetails);
        return false;
      }

      seleniumScriptLogger.Log($"Variable {name} not found in current scope, trying parent", SeleniumScriptLogLevel.InterpreterDetails);
      return parent?.SetVariable(name, value) ?? false;
    }

    public void AddFunction(string name, Function function)
    {
      seleniumScriptLogger.Log($"Adding function {name}", SeleniumScriptLogLevel.InterpreterDetails);
      if (functions.ContainsKey(name))
      {
        throw new SeleniumScriptVisitorException($"Redeclaration of {name} within current scope");
      }

      functions.Add(name, function);
    }

    public Function ResolveFunction(string name)
    {
      seleniumScriptLogger.Log($"Resolving function {name}", SeleniumScriptLogLevel.InterpreterDetails);
      if (functions.ContainsKey(name))
      {
        return functions[name];
      }

      if (scopeType == StackFrameScope.Method || scopeType == StackFrameScope.Global)
      {
        seleniumScriptLogger.Log($"Function {name} could not be resolved", SeleniumScriptLogLevel.InterpreterDetails);
        return null;
      }

      seleniumScriptLogger.Log($"Function {name} not found in current scope, trying parent", SeleniumScriptLogLevel.InterpreterDetails);
      return parent?.ResolveFunction(name);
    }

    public string ResolveVariable(string name)
    {
      seleniumScriptLogger.Log($"Resolving variable {name}", SeleniumScriptLogLevel.InterpreterDetails);
      if (variables.ContainsKey(name))
      {
        return variables[name];
      }

      if (scopeType == StackFrameScope.Method || scopeType == StackFrameScope.Global)
      {
        seleniumScriptLogger.Log($"Varriable {name} could not be resolved", SeleniumScriptLogLevel.InterpreterDetails);
        return null;
      }

      seleniumScriptLogger.Log($"Variable {name} not found in current scope, trying parent", SeleniumScriptLogLevel.InterpreterDetails);
      return parent?.ResolveVariable(name);
    }
  }
}
