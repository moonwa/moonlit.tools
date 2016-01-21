using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Moonlit.PatternDesign;
using Moonlit.Reflection;

namespace Moonlit.Tools.ReflectionExtends
{
    /// <summary>
    /// 命令 : Decorator
    /// </summary>
    [Command("Decorator", "generate decorator code for class")]
    internal class Decorator : ICommand
    {
        public Decorator()
        {
            Access = "public";
        }


        [Parameter(Description = "访问权限 - public")]
        public string Access { get; set; }

        [Parameter(Description = "程序集", Required = true)]
        public string AssemblyFile { get; set; }

        [Parameter(Description = "输出文件", Required = true)]
        public string Output { get; set; }

        [Parameter(Description = "接口类型", Required = true)]
        public string TypeName { get; set; }

        [Parameter(Description = "是否虚函数")]
        public bool Virtual { get; set; }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        #region ICommand Members

        public async Task<int> Execute()
        {
            string typename = TypeName;
            Assembly assembly = Assembly.LoadFile(Path.GetFullPath(AssemblyFile));
            Type t = assembly.GetType(typename, false);
            if (t == null)
            {
                throw new UsageErrorException(string.Format("指定类型 {0} 不存在", typename));
            }
            DecoratorBuilder builder = null;
            if (t.IsInterface)
            {
                builder = new InterfaceDecoratorBuilder(t);
            }
            if (builder != null)
            {
                builder.Namespace = "xxx";
                MemberAttributes attr = CreateMemberAttributes();
                builder.MethodAttributes = attr;
                CodeCompileUnit targetUnit = builder.Build();
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                var options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                using (var sourceWriter = new StreamWriter(Output))
                {
                    provider.GenerateCodeFromCompileUnit(
                        targetUnit, sourceWriter, options);
                }
            }
            return 0;
        }

        #endregion

        private MemberAttributes CreateMemberAttributes()
        {
            MemberAttributes attr = default(MemberAttributes);
            if (!Virtual)
            {
                attr |= MemberAttributes.Final;
            }
            attr |= EnumDescriptor.Parse<MemberAttributes>(Access);
            return attr;
        }
    }
}

public interface IA
{
    string GetA(string name);
    void Echo(string a, string b);
}