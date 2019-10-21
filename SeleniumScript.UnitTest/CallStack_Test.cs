namespace SeleniumScript.UnitTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Moq;
  using SeleniumScript.Factories;
  using SeleniumScript.Implementation;
  using SeleniumScript.Implementation.DataModel;
  using SeleniumScript.Implementation.Enums;
  using SeleniumScript.Interfaces;
  using SeleniumScript.Interpreter.Enums;

  [TestClass]
  public class CallStack_Test
  {
    private readonly Mock<ISeleniumScriptLogger> seleniumScriptLogger = new Mock<ISeleniumScriptLogger>();

    [TestMethod]
    public void Can_Resolve_Function_From_Global_Scope()
    {
      var callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger.Object);

      callStack.Current.AddFunction("name", new Function("name", ReturnType.String));

      var function = callStack.ResolveFunction("name");

      Assert.AreEqual("name", function.Name);
      Assert.AreEqual("string", function.ReturnType);
    }

    [TestMethod]
    public void Can_Resolve_Variable_From_Local_Scope()
    {
      var callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger.Object);

      callStack.Current.AddVariable("global", ReturnType.String, "globalvalue");

      callStack.Push(StackFrameScope.Local);

      callStack.Current.AddVariable("name", ReturnType.String, "value");

      var variable = callStack.ResolveVariable("name");

      Assert.AreEqual("value", variable);

      var globalValue = callStack.ResolveVariable("global");

      Assert.AreEqual("globalvalue", globalValue);
    }

    [TestMethod]
    public void Can_Resolve_Variable_From_Local_Scope_Then_Pop_To_Global_Scope()
    {
      var callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger.Object);

      callStack.Current.AddVariable("global", ReturnType.String, "globalvalue");

      callStack.Push(StackFrameScope.Local);

      callStack.Current.AddVariable("name", ReturnType.String, "value");

      var variable = callStack.ResolveVariable("name");

      Assert.AreEqual("value", variable);

      callStack.Pop();

      var globalValue = callStack.ResolveVariable("global");

      Assert.AreEqual("globalvalue", globalValue);
    }

    [TestMethod]
    public void Will_Not_Go_Above_Method_Scope_Goes_To_Global_Scope()
    {
      var callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger.Object);

      callStack.Current.AddVariable("global1", ReturnType.String, "globalvalue1");
      callStack.Current.AddVariable("global2", ReturnType.String, "globalvalue2");
      callStack.Current.AddVariable("FindThis", ReturnType.String, "Correct");

      callStack.Push(StackFrameScope.Method);

      callStack.Current.AddVariable("name1", ReturnType.String, "value1");
      callStack.Current.AddVariable("name2", ReturnType.String, "value2");
      callStack.Current.AddVariable("FindThis", ReturnType.String, "Incorrect");

      callStack.Push(StackFrameScope.Method);

      callStack.Current.AddVariable("name1", ReturnType.String, "value1");
      callStack.Current.AddVariable("name2", ReturnType.String, "value2");

      callStack.Push(StackFrameScope.Local);

      callStack.Current.AddVariable("name1", ReturnType.String, "value1");
      callStack.Current.AddVariable("name2", ReturnType.String, "value2");

      var variable = callStack.ResolveVariable("FindThis");

      Assert.AreEqual("Correct", variable);
    }

  }
}
