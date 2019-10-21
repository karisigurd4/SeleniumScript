namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Implementation.DataModel;
  using SeleniumScript.Interpreter.Enums;

  public interface IStackFrameHandler
  {
    void AddVariable(string name, ReturnType type, string value = null);
    bool SetVariable(string name, string value);
    void AddFunction(string name, Function function);
    Function ResolveFunction(string name);
    Variable ResolveVariable(string name);
  }
}
