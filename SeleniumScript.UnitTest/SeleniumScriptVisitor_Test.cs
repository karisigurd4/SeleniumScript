namespace SeleniumScript.UnitTest
{
  using Antlr4.Runtime;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Moq;
  using SeleniumScript.Grammar;
  using SeleniumScript.Implementation;
  using SeleniumScript.Interfaces;
  using System;
  using System.Diagnostics;

  [TestClass]
  public class SeleniumScriptVisitor_Test
  {
    private readonly Mock<ISeleniumScriptWebDriver> webDriver = new Mock<ISeleniumScriptWebDriver>();

    private WebDriverOperationLog lastOperation;

    [TestInitialize]
    public void Initialize()
    {
      webDriver.Setup(x => x.Click(It.IsAny<string>(), It.IsAny<string>()))
        .Callback((string xPath, string elementDescription) => lastOperation = new WebDriverOperationLog()
        {
          Arguments = new string[] { xPath, elementDescription },
          OperationType = "Click"
        });

      webDriver.Setup(x => x.SendKeys(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Callback((string xPath, string data, string elementDescription) => lastOperation = new WebDriverOperationLog()
        {
          Arguments = new string[] { xPath, data, elementDescription },
          OperationType = "SendKeys"
        });

      webDriver.Setup(x => x.WaitForSeconds(It.IsAny<int>()))
        .Callback((int numberOfSeconds) => lastOperation = new WebDriverOperationLog()
        {
          Arguments = new string[] { numberOfSeconds.ToString() },
          OperationType = "Wait"
        });

      webDriver.Setup(x => x.NavigateTo(It.IsAny<string>()))
        .Callback((string url) => lastOperation = new WebDriverOperationLog()
        {
          Arguments = new string[] { url },
          OperationType = "NavigateTo"
        });

      webDriver.Setup(x => x.GetElementText(It.IsAny<string>(), It.IsAny<string>()))
        .Returns("Test data")
        .Callback((string xPath, string elementDescription) => lastOperation = new WebDriverOperationLog()
        {
          OperationType = "GetElementText",
          Arguments = new string[] { xPath, elementDescription }
        });

      webDriver.Setup(x => x.GetUrl())
        .Returns("Test URL")
        .Callback(() => lastOperation = new WebDriverOperationLog()
        {
          OperationType = "GetUrl",
          Arguments = new string[] { }
        });
    }

    [TestMethod]
    public void Can_Declare_And_Assign_String_Variable()
    {
      var visitor = VisitScript("string stringVariable = \"Windows 10\";");
      Assert.AreEqual("Windows 10", visitor.DeclaredVariables["stringVariable"]);
    }

    [TestMethod]
    public void Can_Assign_String_Variable_To_Variable()
    {
      var visitor = VisitScript("string firstVariable = \"Windows 10\"; string secondVariable = firstVariable;");
      Assert.AreEqual("Windows 10", visitor.DeclaredVariables["firstVariable"]);
      Assert.AreEqual("Windows 10", visitor.DeclaredVariables["secondVariable"]);
    }

    [TestMethod]
    public void Can_Perform_Operation_Click()
    {
      var visitor = VisitScript("Click(\"xPath\", \"description\");");
      Assert.AreEqual("Click", lastOperation.OperationType);
      Assert.AreEqual("xPath", lastOperation.Arguments[0]);
      Assert.AreEqual("description", lastOperation.Arguments[1]);
    }

    [TestMethod]
    public void Can_Perform_Operation_Click_Without_Description()
    {
      var visitor = VisitScript("Click(\"xPath\");");
      Assert.AreEqual("Click", lastOperation.OperationType);
      Assert.AreEqual("xPath", lastOperation.Arguments[0]);
      Assert.AreEqual("", lastOperation.Arguments[1]);
    }

    [TestMethod]
    public void Can_Perform_Operation_SendKeys()
    {
      var visitor = VisitScript("SendKeys(\"xPath\", \"data\", \"description\");");
      Assert.AreEqual("SendKeys", lastOperation.OperationType);
      Assert.AreEqual("xPath", lastOperation.Arguments[0]);
      Assert.AreEqual("data", lastOperation.Arguments[1]);
      Assert.AreEqual("description", lastOperation.Arguments[2]);
    }

    [TestMethod]
    public void Can_Perform_Operation_SendKeys_Without_Description()
    {
      var visitor = VisitScript("SendKeys(\"xPath\", \"data\");");
      Assert.AreEqual("SendKeys", lastOperation.OperationType);
      Assert.AreEqual("xPath", lastOperation.Arguments[0]);
      Assert.AreEqual("data", lastOperation.Arguments[1]);
      Assert.AreEqual("", lastOperation.Arguments[2]);
    }

    [TestMethod]
    public void Can_Perform_Operation_Wait()
    {
      var visitor = VisitScript("Wait(10);");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Perform_Operation_NavigateTo()
    {
      var visitor = VisitScript("NavigateTo(\"Url\");");
      Assert.AreEqual("NavigateTo", lastOperation.OperationType);
      Assert.AreEqual("Url", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Assign_Variable_To_GetElementText()
    {
      var visitor = VisitScript("string elementText = GetElementText(\"xpath\", \"description\");");
      Assert.AreEqual("GetElementText", lastOperation.OperationType);
      Assert.AreEqual("xpath", lastOperation.Arguments[0]);
      Assert.AreEqual("description", lastOperation.Arguments[1]);
      Assert.AreEqual("Test data", visitor.DeclaredVariables["elementText"]);
    }

    [TestMethod]
    public void Can_Assign_Variable_To_GetElementText_Without_Description()
    {
      var visitor = VisitScript("string elementText = GetElementText(\"xpath\");");
      Assert.AreEqual("GetElementText", lastOperation.OperationType);
      Assert.AreEqual("xpath", lastOperation.Arguments[0]);
      Assert.AreEqual("", lastOperation.Arguments[1]);
      Assert.AreEqual("Test data", visitor.DeclaredVariables["elementText"]);
    }

    [TestMethod]
    public void Can_Assign_Variable_To_GetUrl()
    {
      var visitor = VisitScript("string elementText = GetUrl();");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
      Assert.AreEqual("Test URL", visitor.DeclaredVariables["elementText"]);
    }

    private SeleniumScriptVisitor VisitScript(string script)
    {
      SeleniumScriptLexer seleniumScriptLexer = new SeleniumScriptLexer(new AntlrInputStream(script));
      SeleniumScriptParser seleniumScriptParser = new SeleniumScriptParser(new CommonTokenStream(seleniumScriptLexer));
      var seleniumScriptLogger = new SeleniumScriptLogger();
      seleniumScriptLogger.OnLogEntryWritten += (log) =>
      {
        if(
                log.LogLevel == Enums.LogLevel.RuntimeError 
            ||  log.LogLevel == Enums.LogLevel.SeleniumError
            ||  log.LogLevel == Enums.LogLevel.SyntaxError
            ||  log.LogLevel == Enums.LogLevel.VisitorError
          )
        {
          throw new Exception(log.Message);
        }
      };
      seleniumScriptParser.AddErrorListener(new SeleniumScriptSyntaxErrorListener(seleniumScriptLogger));
      var visitor = new SeleniumScriptVisitor(webDriver.Object, seleniumScriptLogger);
      visitor.Visit(seleniumScriptParser.executionUnit());
      return visitor;
    }
  }
}
