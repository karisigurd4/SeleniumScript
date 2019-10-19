namespace SeleniumScript.UnitTest
{
  using Antlr4.Runtime;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Moq;
  using SeleniumScript.Enums;
  using SeleniumScript.Factories;
  using SeleniumScript.Grammar;
  using SeleniumScript.Implementation;
  using SeleniumScript.Interfaces;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;

  [TestClass]
  public class SeleniumScriptInterpreter_Test
  {
    private readonly Mock<ISeleniumScriptWebDriver> webDriver = new Mock<ISeleniumScriptWebDriver>();

    private List<string> scriptLogOutput = new List<string>();
    private WebDriverOperationLog lastOperation;
    private CallStack callStack;

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
      Assert.AreEqual("Windows 10", callStack.ResolveVariable("stringVariable"));
    }

    [TestMethod]
    public void Can_Assign_String_Variable_To_Variable()
    {
      var visitor = VisitScript("string firstVariable = \"Windows 10\"; string secondVariable = firstVariable;");
      Assert.AreEqual("Windows 10", callStack.ResolveVariable("firstVariable"));
      Assert.AreEqual("Windows 10", callStack.ResolveVariable("secondVariable"));
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
    public void Can_Perform_Operation_Log()
    {
      var visitor = VisitScript("Log(\"Log output!\");");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("Log output!", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_LGT_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (5 > 4) { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_Variable_If_LGT_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("int a = 5; int b = 4; if (a > b) { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\") { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_Integer_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (3 == 3) { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_AND_AND_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\" && \"other\" == \"other\" && \"this\" == \"this\") { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_AND_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\" && \"other\" == \"other\") { Wait(10); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_AND_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"notsame\" && \"other\" == \"other\") { Wait(10); }");
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_AND_AND_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"notsame\" && \"other\" == \"other\" && \"this\" == \"this\") { Wait(10); }");
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_If_FALSE_AND_FALSE_AND_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\" && \"other\" == \"notother\" && \"this\" == \"notthis\") { Wait(10); }");
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_If_AND_False_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\" && \"other\" == \"notother\") { Log(\"Log output!\"); }");
      Assert.AreNotEqual("Log", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_Else_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"same\") { Wait(10); } else { NavigateTo(\"Url\"); }");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_Else_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"notsame\") { Wait(10); } else { NavigateTo(\"Url\"); }");
      Assert.AreEqual("NavigateTo", lastOperation.OperationType);
      Assert.AreEqual("Url", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_ElseIf_If_Else_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""same"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
else 
{ 
  NavigateTo(""Url""); 
}
");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
    }

    [TestMethod]
    public void Can_Evaluate_True_ElseIf_If_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""same"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
    }

    [TestMethod]
    public void Can_Evaluate_True_ElseIf_If_ElseIf_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""same"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
    }

    [TestMethod]
    public void Can_Evaluate_If_True_ElseIf_ElseIf_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""same"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
else if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_ElseIf_Else_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""same"") 
{ 
  Wait(10); 
} 
else if (""notsame"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
else 
{ 
  NavigateTo(""Url""); 
}
");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_If_ElseIfCondition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""same"") 
{ 
  Wait(10); 
} 
else if (""notsame"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
");
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("10", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_True_Else_If_ElseIf_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""notsame"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
else 
{ 
  NavigateTo(""Url""); 
}
");
      Assert.AreEqual("NavigateTo", lastOperation.OperationType);
      Assert.AreEqual("Url", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_ElseIf_Condition()
    {
      var visitor = VisitScript(
@"
if (""same"" == ""notsame"") 
{ 
  Wait(10); 
} 
else if (""notsame"" == ""same"") 
{ 
  string elementText = GetUrl(); 
} 
");
      lastOperation = new WebDriverOperationLog();
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_LGT_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (4 > 6) { Wait(10); }");
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
    }

    [TestMethod]
    public void Can_Evaluate_False_If_Condition()
    {
      lastOperation = new WebDriverOperationLog();
      var visitor = VisitScript("if (\"same\" == \"notsame\") { Wait(10); }");
      Assert.AreNotEqual("Wait", lastOperation.OperationType);
      Assert.IsNull(lastOperation.Arguments);
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
      Assert.AreEqual("Test data", callStack.ResolveVariable("elementText"));
    }

    [TestMethod]
    public void Can_Assign_Variable_To_GetElementText_Without_Description()
    {
      var visitor = VisitScript("string elementText = GetElementText(\"xpath\");");
      Assert.AreEqual("GetElementText", lastOperation.OperationType);
      Assert.AreEqual("xpath", lastOperation.Arguments[0]);
      Assert.AreEqual("", lastOperation.Arguments[1]);
      Assert.AreEqual("Test data", callStack.ResolveVariable("elementText"));
    }

    [TestMethod]
    public void Can_Assign_Variable_To_GetUrl()
    {
      var visitor = VisitScript("string elementText = GetUrl();");
      Assert.AreEqual("GetUrl", lastOperation.OperationType);
      Assert.AreEqual("Test URL", callStack.ResolveVariable("elementText"));
    }

    [TestMethod]
    public void Can_Declare_Function()
    {
      var visitor = VisitScript("string MyFunc() { Log(\"This is a function\"); }");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
    }

    [TestMethod]
    public void Can_Call_Function()
    {
      var visitor = VisitScript(
        @"
string MyFunc() 
{ 
  Log(""This is a function""); 
  Wait(1); 
} 

MyFunc();
");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("1", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Call_Function_With_Parameter()
    {
      scriptLogOutput.Clear();

      var visitor = VisitScript(
        @"
string MyFunc(string s) 
{ 
  Log(s); 
  Wait(1); 
} 

MyFunc(""This is a function"");
");
      Assert.AreEqual("This is a function", scriptLogOutput.First());
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("1", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Call_Function_With_Reference_Parameter()
    {
      scriptLogOutput.Clear();

      var visitor = VisitScript(
        @"
string MyFunc(string parameter) 
{ 
  Log(parameter); 
  Wait(1); 
} 

string global = ""This is a function""; 
MyFunc( global );
");
      Assert.AreEqual("This is a function", scriptLogOutput.First());
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Wait", lastOperation.OperationType);
      Assert.AreEqual("1", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Call_Function_With_Reference_Parameter_Returns_Same()
    {
      scriptLogOutput.Clear();

      var visitor = VisitScript(
        @"
string MyFunc(string parameter) 
{ 
  Log(parameter); 
  Wait(1); 
  return ""Changed"";
} 

string global = ""This is a function""; 
global = MyFunc(global);

Log(global);
");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("Changed", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Return_Data_From_Function()
    {
      var visitor = VisitScript(
        @"
string MyFunc() 
{ 
  string functionVariable = ""This variable contains data!"";
  Log(""This is a function""); 
  Wait(1); 
  return functionVariable;
} 

string returnedVariable = MyFunc();

Log(returnedVariable);
");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("This variable contains data!", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Return_Data_From_Function_With_Three_Parameters()
    {
      var visitor = VisitScript(
        @"
string MyFunc(string testString, string toLog, int waitTime) 
{ 
  string functionVariable = testString;
  Log(toLog); 
  Wait(waitTime); 
  return functionVariable;
} 

string returnedVariable = MyFunc(""hello world!"", ""logthis"", 5);

Log(returnedVariable);
");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("hello world!", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Return_Data_From_Function_With_One_Parameter()
    {
      var visitor = VisitScript(
@"
string MyFunc(string testString) 
{ 
  string functionVariable = testString;
  return functionVariable;
} 

string returnedVariable = MyFunc(""hello world!"");

Log(returnedVariable);
");
      Assert.IsNotNull(callStack.ResolveFunction("MyFunc"));
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("hello world!", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Define_Arbitrary_Local_Scopes()
    {
      var visitor = VisitScript(
@"
string variable = ""incorrect1"";
{
  string variable = ""incorrect2"";
  {
    string variable = ""incorrect3"";
    {
      string variable = ""Correct!"";
      Log(variable);
    }
  }
}

");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("Correct!", lastOperation.Arguments[0]);

      visitor = VisitScript(
@"
string variable = ""incorrect1"";
{
  string variable = ""incorrect2"";
  {
    string variable = ""Correct!"";
    {
      string variable = ""incorrect3"";
    }
    Log(variable);
  }
}

");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("Correct!", lastOperation.Arguments[0]);

      visitor = VisitScript(
@"
string variable = ""incorrect1"";
{
  string variable = ""incorrect2"";
  {
    string variable = ""Correct!"";
    {
      string variable = ""incorrect3"";
    }

    if (""same"" == ""same"") 
    {
      Log(variable);
    }
  }
}

");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("Correct!", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Run_For_Loop()
    {
      scriptLogOutput.Clear();

      var visitor = VisitScript(
@"
for (int i = 0; i < 10; i++)
{
  Log(i);
}
");
      Assert.AreEqual(10, scriptLogOutput.Count);

      for (int i = 0; i < 10; i++)
      {
        Assert.AreEqual(i.ToString(), scriptLogOutput[i]);
      }
    }

    [TestMethod]
    public void Can_Call_Function_From_For_loop()
    {
      scriptLogOutput.Clear();

      var visitor = VisitScript(
@"
string MyFunc(string o) 
{
  Log(o);
}

for (int i = 0; i < 10; i++)
{
  string batman = ""batman"";
  MyFunc(batman);
}
");
      Assert.AreEqual(10, scriptLogOutput.Count);

      for (int i = 0; i < 10; i++)
      {
        Assert.AreEqual("batman", scriptLogOutput[i]);
      }
    }

    [TestMethod]
    public void Can_Use_Unary_Increment_Operator()
    {
      var visitor = VisitScript(
@"
int i = 10;
i++;
Log(i);
");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("11", lastOperation.Arguments[0]);
    }

    [TestMethod]
    public void Can_Use_Unary_Decrement_Operator()
    {
      var visitor = VisitScript(
@"
int i = 10;
i--;
Log(i);
");
      Assert.AreEqual("Log", lastOperation.OperationType);
      Assert.AreEqual("9", lastOperation.Arguments[0]);
    }

    private SeleniumScriptInterpreter VisitScript(string script)
    {
      SeleniumScriptLexer seleniumScriptLexer = new SeleniumScriptLexer(new AntlrInputStream(script));
      SeleniumScriptParser seleniumScriptParser = new SeleniumScriptParser(new CommonTokenStream(seleniumScriptLexer));
      var seleniumScriptLogger = new SeleniumScriptLogger();
      seleniumScriptLogger.OnLogEntryWritten += (log) =>
      {
        if(
                log.LogLevel == Enums.SeleniumScriptLogLevel.RuntimeError 
            ||  log.LogLevel == Enums.SeleniumScriptLogLevel.WebDriver
            ||  log.LogLevel == Enums.SeleniumScriptLogLevel.SyntaxError
            ||  log.LogLevel == Enums.SeleniumScriptLogLevel.VisitorError
          )
        {
          throw new Exception(log.Message);
        }
        else if (log.LogLevel == Enums.SeleniumScriptLogLevel.Script)
        {
          lastOperation = new WebDriverOperationLog()
          {
            Arguments = new string[] { log.Message },
            OperationType = "Log"
          };

          scriptLogOutput.Add(log.Message);
        }
        else
        {
          Debug.WriteLine($"{log.TimeStamp} [{log.LogLevel}] - {log.Message}");
        }
      };
      seleniumScriptParser.AddErrorListener(new SeleniumScriptSyntaxErrorListener());
      callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger);
      var visitor = new SeleniumScriptInterpreter(webDriver.Object, callStack, seleniumScriptLogger);
      visitor.Visit(seleniumScriptParser.executionUnit());
      return visitor;
    }
  }
}
