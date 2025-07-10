using UglyToad.PdfPig;
using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.VisualBasic;

[McpServerToolType]
public static class EchoTools
{
   [McpServerTool, Description("Echoes the input back to the client. With a prefix from the server.")]
   public static string Echo(string message)
   {
      Console.WriteLine($"Echoing: {message}");
      return $"hello from c#: {message}";
   }

   [McpServerTool, Description("Echoes in reverse the message sent by the client. With a prefix from the server.")]
   public static string ReverseEcho(string message)
   {
      string reversed = new string(message.Reverse().ToArray());
      Console.WriteLine($"Echoing in reverse: {reversed}");
      return " Hello from c# reverse: " + reversed;
   }


}

[McpServerResourceType]
public static class MyCvResources
{
   // get root directory of the project
   private static  string _rootDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
   
   public static PdfDocument GetMyCv()
   {
      if(_rootDir == null)
      {
       _rootDir = Directory.GetCurrentDirectory();
      }
      
      // Assuming the CV is stored in the same directory as the executable
      return PdfDocument.Open(System.IO.Path.Combine(_rootDir, "cv.pdf"));
   }
}
