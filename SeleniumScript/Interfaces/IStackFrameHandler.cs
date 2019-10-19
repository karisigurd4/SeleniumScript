namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Implementation.DataModel;

  public interface IStackFrameHandler
  {
    void AddVariable(string name, string value = null);
    bool SetVariable(string name, string value);
    void AddFunction(string name, Function function);
    Function ResolveFunction(string name);
    string ResolveVariable(string name);
  }
}
