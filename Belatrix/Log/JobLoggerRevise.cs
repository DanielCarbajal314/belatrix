using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public enum LoggingConfigurations
    {
        LogToFile = 1,
        LogToConsole = 2,
        LogToDatabase = 3,
        LogMessage = 4,
        LogWarning = 5,
        LogError = 6
    }

    public enum MessageType
    {
        Message = 1,
        Warning = 2,
        Error = 3
    }

    public class JobLoggerRevise
    {
        private bool _logToFile;
        private bool _logToConsole;
        private bool _logToDatabase;
        private bool _logMessage;
        private bool _logWarning;
        private bool _logError;
        public string FileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
            }
        }

        public JobLoggerRevise(params LoggingConfigurations[] args)
        {
            this._logToFile = args.Contains(LoggingConfigurations.LogToFile);
            this._logToConsole = args.Contains(LoggingConfigurations.LogToConsole);
            this._logToDatabase = args.Contains(LoggingConfigurations.LogToDatabase);
            this._logMessage = args.Contains(LoggingConfigurations.LogMessage);
            this._logWarning = args.Contains(LoggingConfigurations.LogWarning);
            this._logError = args.Contains(LoggingConfigurations.LogError);
            if (!this._logToFile && !this._logToConsole && !this._logToDatabase)
            {
                throw new Exception("Invalidconfiguration : You must specified at least one logging data output (File, Console or Database)");
            }
            if ((!this._logError && !this._logMessage && !this._logWarning))
            {
                throw new Exception("Invalidconfiguration :At least one type of event must be specified to be logged (Error, Warning or Message)");
            }
        }

        public void LogMessage(string message, MessageType type)
        {
            if (!this.ShouldItBeLog(type)) return;
            if (String.IsNullOrWhiteSpace(message)) return; // I wouldn't do that
            message.Trim();
            message = DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss") + ": " + message;
            if (_logToDatabase)
            {
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("Insert into Log Values('" + message + "'," + (int)type + ")", connection);
                        command.ExecuteNonQuery();
                    }
            }
            if (_logToFile)
            {
                using (StreamWriter writer = File.AppendText(this.FileName))
                {
                    writer.WriteLine(message);
                }
            }
            if (_logToConsole)
            {
                Console.ForegroundColor = this.ConsoleColorOfType(type);
                Console.WriteLine(message);
            }
        }

        private bool ShouldItBeLog(MessageType type)
        {
            return MessageType.Message == type ? this._logMessage :
                   MessageType.Warning == type ? this._logWarning : this._logError;
        }

        private ConsoleColor ConsoleColorOfType(MessageType type)
        {
            return MessageType.Message == type ? ConsoleColor.White :
                   MessageType.Warning == type ? ConsoleColor.Yellow : ConsoleColor.Red;
        }
    }
}
