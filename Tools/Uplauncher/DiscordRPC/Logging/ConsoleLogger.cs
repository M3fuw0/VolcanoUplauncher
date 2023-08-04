﻿using System;

namespace DiscordRPC.Logging
{
	/// <summary>
	///     Logs the outputs to the console using <see cref="Console.WriteLine()" />
	/// </summary>
	public class ConsoleLogger : ILogger
    {
	    /// <summary>
	    ///     Creates a new instance of a Console Logger.
	    /// </summary>
	    public ConsoleLogger()
        {
            Level = LogLevel.Info;
            Coloured = false;
        }

	    /// <summary>
	    ///     Creates a new instance of a Console Logger
	    /// </summary>
	    /// <param name="level">The log level</param>
	    public ConsoleLogger(LogLevel level)
            : this()
        {
            Level = level;
        }

	    /// <summary>
	    ///     Creates a new instance of a Console Logger with a set log level
	    /// </summary>
	    /// <param name="level">The log level</param>
	    /// <param name="coloured">Should the logs be in colour?</param>
	    public ConsoleLogger(LogLevel level, bool coloured)
        {
            Level = level;
            Coloured = coloured;
        }

	    /// <summary>
	    ///     Should the output be coloured?
	    /// </summary>
	    public bool Coloured { get; set; }

	    /// <summary>
	    ///     A alias too <see cref="Coloured" />
	    /// </summary>
	    [Obsolete("Use Coloured")]
        public bool Colored
        {
            get => Coloured;
            set => Coloured = value;
        }

	    /// <summary>
	    ///     The level of logging to apply to this logger.
	    /// </summary>
	    public LogLevel Level { get; set; }

	    /// <summary>
	    ///     Informative log messages
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="args"></param>
	    public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;

            if (Coloured) Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("TRACE: " + message, args);
        }

	    /// <summary>
	    ///     Informative log messages
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="args"></param>
	    public void Info(string message, params object[] args)
        {
            if (Level > LogLevel.Info) return;

            if (Coloured) Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("INFO: " + message, args);
        }

	    /// <summary>
	    ///     Warning log messages
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="args"></param>
	    public void Warning(string message, params object[] args)
        {
            if (Level > LogLevel.Warning) return;

            if (Coloured) Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARN: " + message, args);
        }

	    /// <summary>
	    ///     Error log messsages
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="args"></param>
	    public void Error(string message, params object[] args)
        {
            if (Level > LogLevel.Error) return;

            if (Coloured) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERR : " + message, args);
        }
    }
}