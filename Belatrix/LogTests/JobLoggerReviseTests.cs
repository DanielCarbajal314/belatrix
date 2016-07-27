using Microsoft.VisualStudio.TestTools.UnitTesting;
using Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Belatrix.Model;

namespace Log.Tests
{
    [TestClass]
    public class JobLoggerReviseTests
    {
        [TestMethod]
        public void WrongConstructor()
        {

            try
            {
                JobLoggerRevise EntryLogger = new JobLoggerRevise();
                Assert.Fail("It allows me to call the constructor empty");
            }
            catch(Exception e)
            {
                if (!e.Message.Contains("Invalidconfiguration"))
                {
                    Assert.Fail("Wrong message on empty contructor");
                }
            }
            try
            {
                JobLoggerRevise BlindLogger = new JobLoggerRevise
                                                                (
                                                                    LoggingConfigurations.LogToDatabase,
                                                                    LoggingConfigurations.LogToConsole,
                                                                    LoggingConfigurations.LogToFile
                                                                );
                Assert.Fail("It allows me to call the constructor with no logging targets");
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Invalidconfiguration"))
                {
                    Assert.Fail("Wrong message on Blind contructor");
                }
            }
            try
            {
                JobLoggerRevise LazyLogger = new JobLoggerRevise
                                                                (
                                                                    LoggingConfigurations.LogWarning,
                                                                    LoggingConfigurations.LogError,
                                                                    LoggingConfigurations.LogMessage
                                                                );
                Assert.Fail("It allows me to call the constructor with no logging outputs");
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Invalidconfiguration"))
                {
                    Assert.Fail("Wrong message on Lazy contructor");
                }
            }
           
        }
        [TestMethod]
        public void ErrorToFile()
        {
            JobLoggerRevise Logger = new JobLoggerRevise(
                                                            LoggingConfigurations.LogError,
                                                            LoggingConfigurations.LogToFile
                                                        );
            File.Delete(Logger.FileName);
            Logger.LogMessage("ErrorTEST_ERROR", MessageType.Error);
            string[] lines = File.ReadAllLines(Logger.FileName);
            File.Delete(Logger.FileName);
            if (lines.Length == 0)
            {
                Assert.Fail("file has no lines written on");
                return;
            }
            if (!lines[0].Contains("ErrorTEST_ERROR"))
            {
                Assert.Fail("Error String wasn't found on file");
                return;
            }
        }
        [TestMethod]
        public void MessageToFile()
        {

            JobLoggerRevise Logger = new JobLoggerRevise(
                                                            LoggingConfigurations.LogMessage,
                                                            LoggingConfigurations.LogToFile
                                                        );
            File.Delete(Logger.FileName);
            Logger.LogMessage("MessageTEST_MESSAGE", MessageType.Message);
            string[] lines = File.ReadAllLines(Logger.FileName);
            File.Delete(Logger.FileName);
            if (lines.Length == 0)
            {
                Assert.Fail("file has no lines written on");
                return;
            }
            if (!lines[0].Contains("MessageTEST_MESSAGE"))
            {
                Assert.Fail("Message String wasn't found on file");
                return;
            }
        }
        [TestMethod]
        public void WarningToFile()
        {
            JobLoggerRevise Logger = new JobLoggerRevise(
                                                            LoggingConfigurations.LogWarning,
                                                            LoggingConfigurations.LogToFile
                                                        );
            File.Delete(Logger.FileName);
            Logger.LogMessage("WarningTEST_WARNING", MessageType.Warning);
            string[] lines = File.ReadAllLines(Logger.FileName);
            File.Delete(Logger.FileName);
            if (lines.Length == 0)
            {
                Assert.Fail("file has no lines written on");
                return;
            }
            if (!lines[0].Contains("WarningTEST_WARNING"))
            {
                Assert.Fail("Warning String wasn't found on file");
                return;
            }
        }
        [TestMethod]
        public void ErrorToDataBase()
        {
            LogEntities db = new LogEntities();
            db.DeleteAllLogs();
            JobLoggerRevise Logger = new JobLoggerRevise(
                                                LoggingConfigurations.LogToDatabase,
                                                LoggingConfigurations.LogError
                                            );
            Logger.LogMessage("ErrorTEST_ERROR", MessageType.Error);
            Belatrix.Model.Log log = db.Log.FirstOrDefault();
            if (!log.descripcion.Contains("ErrorTEST_ERROR"))
            {
                Assert.Fail("Error String wasn't found on DataBase");
            }
            db.DeleteAllLogs();
        }
        [TestMethod]
        public void WarningToDataBase()
        {
            LogEntities db = new LogEntities();
            db.DeleteAllLogs();
            JobLoggerRevise Logger = new JobLoggerRevise(
                                                LoggingConfigurations.LogToDatabase,
                                                LoggingConfigurations.LogWarning
                                            );
            Logger.LogMessage("WarningTEST_WARNING", MessageType.Warning);
            Belatrix.Model.Log log = db.Log.FirstOrDefault();
            if (!log.descripcion.Contains("WarningTEST_WARNING"))
            {
                Assert.Fail("Warning String wasn't found on DataBase");
            }
            db.DeleteAllLogs();
        }
        [TestMethod]
        public void MessageToDataBase()
        {
            LogEntities db = new LogEntities();
            db.DeleteAllLogs();
            JobLoggerRevise Logger = new JobLoggerRevise(
                                                LoggingConfigurations.LogToDatabase,
                                                LoggingConfigurations.LogMessage
                                            );
            Logger.LogMessage("MessageTEST_MESSAGE", MessageType.Message);
            Belatrix.Model.Log log = db.Log.FirstOrDefault();
            if (!log.descripcion.Contains("MessageTEST_MESSAGE"))
            {
                Assert.Fail("Message String wasn't found on DataBase");
            }
            db.DeleteAllLogs();
        }
        [TestMethod]
        public void ErrorToConsole()
        {
            TextWriter originalSTD_OUT = Console.Out;
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                JobLoggerRevise Logger = new JobLoggerRevise(
                                    LoggingConfigurations.LogToConsole,
                                    LoggingConfigurations.LogError
                                );
                Logger.LogMessage("ErrorTEST_ERROR", MessageType.Error);
                writer.Flush();
                string result = writer.GetStringBuilder().ToString();
                if (!result.Contains("ErrorTEST_ERROR"))
                {
                    Assert.Fail("Error String wasn't found on Console");
                }
            }
            Console.SetOut(originalSTD_OUT);
        }
        [TestMethod]
        public void WarningToConsole()
        {
            TextWriter originalSTD_OUT = Console.Out;
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                JobLoggerRevise Logger = new JobLoggerRevise(
                                    LoggingConfigurations.LogToConsole,
                                    LoggingConfigurations.LogWarning
                                );
                Logger.LogMessage("WarningTEST_WARNING", MessageType.Warning);
                writer.Flush();
                string result = writer.GetStringBuilder().ToString();
                if (!result.Contains("WarningTEST_WARNING"))
                {
                    Assert.Fail("Warning String wasn't found on Console");
                }
            }
            Console.SetOut(originalSTD_OUT);
        }
        [TestMethod]
        public void MessageToConsole()
        {
            TextWriter originalSTD_OUT = Console.Out;
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                JobLoggerRevise Logger = new JobLoggerRevise(
                                    LoggingConfigurations.LogToConsole,
                                    LoggingConfigurations.LogMessage
                                );
                Logger.LogMessage("MessageTEST_MESSAGE", MessageType.Message);
                writer.Flush();
                string result = writer.GetStringBuilder().ToString();
                if (!result.Contains("MessageTEST_MESSAGE"))
                {
                    Assert.Fail("Message String wasn't found on Console");
                }
            }
            Console.SetOut(originalSTD_OUT);
        }
        [TestMethod]
        public void All()
        {
            JobLoggerRevise Logger = new JobLoggerRevise
                                (
                                    LoggingConfigurations.LogError,
                                    LoggingConfigurations.LogMessage,
                                    LoggingConfigurations.LogWarning,
                                    LoggingConfigurations.LogToDatabase,
                                    LoggingConfigurations.LogToConsole,
                                    LoggingConfigurations.LogToFile
                                );
            TextWriter originalSTD_OUT = Console.Out;
            File.Delete(Logger.FileName);
            LogEntities db = new LogEntities();
            db.DeleteAllLogs();
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                Logger.LogMessage("MessageTEST_MESSAGE", MessageType.Message);
                Logger.LogMessage("WarningTEST_WARNING", MessageType.Warning);
                Logger.LogMessage("ErrorTEST_ERROR", MessageType.Error);
                writer.Flush();
                string result = writer.GetStringBuilder().ToString();
                if (
                     !(
                         result.Contains("MessageTEST_MESSAGE") &&
                         result.Contains("WarningTEST_WARNING") &&
                         result.Contains("ErrorTEST_ERROR") 
                      )
                   )
                {
                    Assert.Fail("There was a missing number of entries on Consoles's log : "+ result);
                }
            }
            Console.SetOut(originalSTD_OUT);


            int NumberOfRegisters=db.Log.Where(x=>
                                                x.descripcion.Contains("MessageTEST_MESSAGE")||
                                                x.descripcion.Contains("WarningTEST_WARNING")||
                                                x.descripcion.Contains("ErrorTEST_ERROR") 
                                              ).Count();
            if (NumberOfRegisters != 3)
            {
                Assert.Fail("There are only "+ NumberOfRegisters+"on database, we need 3!!");
            }
            string[] lines = File.ReadAllLines(Logger.FileName);
            File.Delete(Logger.FileName);
            if (lines.Length == 0)
            {
                Assert.Fail("file has no lines written on");
                return;
            }
            if (lines.Length != 3)
            {
                Assert.Fail("file has only "+lines.Length+" lines written on");
                return;
            }
            if (!lines[0].Contains("MessageTEST_MESSAGE"))
            {
                Assert.Fail("Message String wasn't found on file");
            }
            if (!lines[1].Contains("WarningTEST_WARNING"))
            {
                Assert.Fail("Warning String wasn't found on file");
            }
            if (!lines[2].Contains("ErrorTEST_ERROR"))
            {
                Assert.Fail("Error String wasn't found on file");
            }

        }
    }
}