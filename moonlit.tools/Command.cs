using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    public abstract class Command
    {
        protected ILogger Logger { get; }
        public Command()
        {
            Logger = new ConsoleLogger();
        }
        /// <summary>
        /// 初始化全局参数解析器
        /// 该参数解析器会使用 [?:Help]为参数值
        /// 其它输入参数都将为目标
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Parser GetGlobalArgumentParser(string[] args)
        {
            var parser = new Parser();
            parser.AddArguments(new HelpParameter(), new DefinitionParameter("debug", "进入调试模式", (LongPrefix)"debug"));
            parser.Parse(args);

            return parser;
        }
        #region CreateCommand


        public static int RunCommand<T>(string[] args) where T : Command
        {
            var command = CreateCommandInstance(typeof(T));
            return command.Execute(args);
        }
        private static Command CreateCommandInstance(Type type)
        {
            Command command = (Command)System.Activator.CreateInstance(type);
            return command;
        }

        private int Execute(string[] args)
        {
            Parser globalParser = GetGlobalArgumentParser(args);

            bool showHelp = globalParser.GetEntity<DefinitionParameter>("help").Defined;

            if (showHelp)
            {
                new Usage(this.GetType(), Logger).Show("");
                return 0;
            }

            if (globalParser.GetEntity<DefinitionParameter>("debug").Defined)
            {
                Logger.InfoLine("starting debug...");
                Debugger.Launch();
            }
            try
            {
                List<string> parameters = globalParser.Targets.ToList();
                Parser parser = CreateParser(this.GetType());
                parser.Parse(parameters.ToArray());
                FillCommand(this, parser);
                return this.Run();
            }
            catch (UsageErrorException usageEx)
            {
                new Usage(this.GetType(), Logger).Show(usageEx.Message);
            }
            catch (Exception ex)
            {
                Logger.WarnLine(ex.ToString());
                return -1;
            }
            return 0;
        }

        protected abstract int Run();


        private static void FillCommand(Command command, Parser parser)
        {
            Type type = command.GetType();
            while (type != null)
            {
                FillCommand(command, type, parser);
                type = type.BaseType;
            }
        }

        private static void FillCommand(Command command, Type commandType, Parser parser)
        {
            Dictionary<string, object> di = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in commandType.GetProperties())
            {
                object[] attrs = propertyInfo.GetCustomAttributes(false);
                var parameterSetter = attrs.OfType<IParameter>().FirstOrDefault();
                di[propertyInfo.Name] = parameterSetter?.GetValue(parser, propertyInfo);
            }
            Newtonsoft.Json.JsonConvert.PopulateObject(Newtonsoft.Json.JsonConvert.SerializeObject(di), command);
        }

        internal static Parser CreateParser(Type commandType)
        {
            var parser = new Parser();

            Type type = commandType;
            while (type != null)
            {
                CreateParser(type, parser);
                type = type.BaseType;
            }
            return parser;
        }

        private static void CreateParser(Type commandType, Parser parser)
        {
            foreach (PropertyInfo property in commandType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                IParameter parameterAttr = property.GetCustomAttributes(true).OfType<IParameter>().FirstOrDefault();

                parameterAttr?.Set(parser, property);
            }
        }

        #endregion
    }
}