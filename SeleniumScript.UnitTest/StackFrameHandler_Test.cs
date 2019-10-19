namespace SeleniumScript.UnitTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Moq;
  using SeleniumScript.Enums;
  using SeleniumScript.Factories;
  using SeleniumScript.Implementation.DataModel;
  using SeleniumScript.Implementation.Enums;
  using SeleniumScript.Interfaces;

  [TestClass]
  public class StackFrameHandler_Test
  {
    private readonly Mock<ISeleniumScriptLogger> seleniumScriptLogger = new Mock<ISeleniumScriptLogger>();
    private readonly IStackFrameHandlerFactory stackFrameHandlerFactory = new StackFrameHandlerFactory();

    [TestInitialize]
    public void Initialize()
    {
      seleniumScriptLogger.Setup(x => x.Log(It.IsAny<string>(), It.IsAny<SeleniumScriptLogLevel>()));
    }

    [TestMethod]
    public void Can_Add_And_Resolve_Variable()
    {
      var stackFrame = stackFrameHandlerFactory.Create(null, StackFrameScope.Global, seleniumScriptLogger.Object);

      stackFrame.AddVariable("name", "value");

      var resolved = stackFrame.ResolveVariable("name");

      Assert.AreEqual("value", resolved);
    }

    [TestMethod]
    public void Can_Add_And_Set_And_Resolve_Variable()
    {
      var stackFrame = stackFrameHandlerFactory.Create(null, StackFrameScope.Global, seleniumScriptLogger.Object);

      stackFrame.AddVariable("name");
      stackFrame.SetVariable("name", "anotherValue");
      var resolved = stackFrame.ResolveVariable("name");

      Assert.AreEqual("anotherValue", resolved);
    }

    [TestMethod]
    public void Can_Add_And_Resolve_Function()
    {
      var stackFrame = stackFrameHandlerFactory.Create(null, StackFrameScope.Global, seleniumScriptLogger.Object);

      stackFrame.AddFunction("name", new Function("name", "string"));

      var resolved = stackFrame.ResolveFunction("name");

      Assert.AreEqual("name", resolved.Name);
      Assert.AreEqual("string", resolved.ReturnType);
    }

    [TestMethod]
    public void Can_Resolve_Variable_From_Parent()
    {
      var methodScope = stackFrameHandlerFactory.Create(null, StackFrameScope.Method, seleniumScriptLogger.Object);
      var localScope = stackFrameHandlerFactory.Create(methodScope, StackFrameScope.Local, seleniumScriptLogger.Object);

      methodScope.AddVariable("name", "value");

      var resolved = localScope.ResolveVariable("name");

      Assert.AreEqual("value", resolved);
    }

    [TestMethod]
    public void Can_Not_Resolve_Variable_Above_Method()
    {
      var firstMethodScope = stackFrameHandlerFactory.Create(null, StackFrameScope.Method, seleniumScriptLogger.Object);
      var secondMethodScope = stackFrameHandlerFactory.Create(firstMethodScope, StackFrameScope.Method, seleniumScriptLogger.Object);
      var localScope = stackFrameHandlerFactory.Create(secondMethodScope, StackFrameScope.Local, seleniumScriptLogger.Object);

      firstMethodScope.AddVariable("name", "value");

      var resolved = localScope.ResolveVariable("name");

      Assert.IsNull(resolved);
    }
  }
}
