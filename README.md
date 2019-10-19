# SeleniumScript
A scripting language abstraction built on top of Selenium WebDriver. Intended to make the process of writing frontend tests quicker and easier to maintain. Any flavour of IWebDriver can be plugged into the component. 

## Usage
```C#
var script = "NavigateTo("https://www.microsoft.com/sv-se/");  Click("//button[@id = 'search']"); ...";

var driver = new ChromeDriver();

using (var seleniumScript = new SeleniumScript(driver))
{
  seleniumScript.Run(script);
}
```

## Language
SeleniumScript provides a number of built-in functions,
* **SendKeys**(*xPath*, *data*, (optional) *description*);
* **Click**(*xPath*, (optional) *description*);
* **GetElementText**(*xPath*, (optional) *description*)
* **GetUrl**()
* **NavigateTo**(*url*);
* **Wait**(*numberOfSeconds*);
* **Log**(*message*)
* **Callback**(*callbackName*);

SeleniumScript automatically creates log output on the SeleniumInfo log level for each of these operations. When the optional description parameters are used it serves to provide more comprehensive and usable diagnostic and monitoring information. 

### Variables
The following variable types are currently supported
* **int**
* **string**

Example usage
```C#
string variableName = "Hello world!";
variableName = "New value!";
string currentUrl = GetUrl();
string elementText = GetElementText("//h1[@id = 'DynamicHeading_productTitle']");
int numberOfSeconds = 3;
Wait(numberOfSeconds);
```  

### Functions
**Simple function definition**
```C#
MyFunc() 
{ 
  Log("This is a function"); 
} 
```

**Parameterized function with a return vlaue**
```C#
string GetTopResultOnGoogle(string searchString)
{
  NavigateTo("https://www.google.com/");
  SendKeys("//input[@name = 'q']", elementText, ""Search bar"");
  Click("//input[@name = 'q']//following::a", elementText, ""Search bar"");
  
  return GetElementText("//div[@class = 'g']", "First search result");
}
```  

### Control flow
**If - else if - else statements**
```C#
if ("same" == "notsame") 
{ 
  Wait(10); 
} 
else if ("same"" == "same") 
{ 
  string elementText = GetUrl(); 
} 
else 
{ 
  NavigateTo("Url"); 
}
```  

**For loops**
```C#
for (int i = 0; i < 10; i++)
{
  Log(i);
}
```  

## Callbacks
You can define any number of custom callback handlers that you can then reference in your scripts. This is useful in situations where you need to let the script trigger an event in your backend. Your registered callback handler can route operations accordingly.

```C#
string script = "Callback(\"callback\");";

string callbackOutput = string.Empty;

using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
{
  seleniumScript.RegisterCallbackHandler("callback", () => { callbackOutput = "Assigned"; });
  seleniumScript.Run(script);
}
```  

## Logging
SeleniumScript has the following levels of logging,
* **Script -** Output generated by script **Log** function calls
* **VisitorDetails -** Diagnostic output from the visitors
* **SeleniumInfo -** Diagnosic output from the Selenium WebDriver abstraction in SeleniumScript
* **SyntaxError -** Syntactic error logs
* **VisitorError -** Visitor errors
* **WebDriverError -** Errors occurring in the Selenium WebDriver abstraction
* **SeleniumError -** Errors occurring in the top level SeleniumScript component
* **RuntimeError -** Errors that might occur in the overlying C# runtime

### Log routing
By default SeleniumScript will not do anything at all with the logs it generates. It is up to the client using SeleniumScript to hook up the **OnLogEntryWritten** event handler and route the logs as they please.

```C#
seleniumScript.OnLogEntryWritten += (log) =>
{
  Debug.WriteLine(log.Message);
};
```

### SeleniumInfo logs
The SeleniumInfo log level will provide an overview of the steps taken in your scripts. This is especially useful for cases like test logs. Below is a formatted example of what the output from a script might look like.

    2019-10-17 21:35:20 [SeleniumInfo] - Navigating driver to https://www.microsoft.com/sv-se/
    2019-10-17 21:35:21 [SeleniumInfo] - Clicking element search button
    2019-10-17 21:35:21 [SeleniumInfo] - Sending keys Windows 10 to search input
    2019-10-17 21:35:22 [SeleniumInfo] - Clicking element First search result
    2019-10-17 21:35:22 [SeleniumInfo] - Operation not successful, retrying...
    2019-10-17 21:35:25 [SeleniumInfo] - Sending keys fakeemail@fakeemail to email address input
    2019-10-17 21:35:26 [SeleniumInfo] - Waiting for 1 seconds
    2019-10-17 21:35:27 [SeleniumInfo] - Clicking element Close dialog button
    2019-10-17 21:35:27 [SeleniumInfo] - Finding text on element Product title
    2019-10-17 21:35:27 [SeleniumInfo] - Navigating driver to https://www.google.com/
    2019-10-17 21:35:27 [SeleniumInfo] - Sending keys Windows 10 Home to Search bar
    2019-10-17 21:35:27 [SeleniumInfo] - Closing and quitting webdriver
