namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Implementation.DataModel;
  using SeleniumScript.Implementation.Enums;

  public interface ICallStack
  {
    IStackFrameHandler Current { get; }
    Function ResolveFunction(string name);
    string ResolveVariable(string name);
    void SetVariable(string name, string value);
    void Pop();
    void Push(StackFrameScope stackFrameScopeType);
  }
}
