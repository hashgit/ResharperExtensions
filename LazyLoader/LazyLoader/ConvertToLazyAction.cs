using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;

namespace LazyLoader
{
    [ContextAction(Description = "Lazy<T> converter",
      Group = "C#",
      Name = "Convert to Lazy<T>")]
    public sealed class ConvertToLazyAction : ContextActionBase
    {
        private readonly ICSharpContextActionDataProvider _provider;
        private ICSharpParameterDeclaration _parameterDeclaration;

        /// <summary>
        /// For languages other than C# any inheritor of <see cref="IContextActionDataProvider"/> can 
        /// be injected in this constructor.
        /// </summary>
        public ConvertToLazyAction(ICSharpContextActionDataProvider provider)
        {
            _provider = provider;
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var parameter = _provider.GetSelectedElement<ICSharpParameterDeclaration>(false, true);
            if (parameter == null || !parameter.IsValid()) return false;

            if (parameter.Type != null)
            {
                var className = parameter.Type.GetLongPresentableName(CSharpLanguage.Instance);
                if (className != null && className.Contains("System.Lazy"))
                    return false;
            }

            _parameterDeclaration = parameter;
            return true;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var lazyTypeElem = TypeElementUtil.GetTypeElementByClrName(new ClrTypeName("System.Lazy`1"), _provider.PsiModule, _provider.PsiModule.GetContextFromModule());
            var lazyType = TypeFactory.CreateType(lazyTypeElem, _parameterDeclaration.Type);
            _parameterDeclaration.SetType(lazyType);

            return null;
        }

        public override string Text
        {
            get { return "Convert to Lazy<T>"; }
        }
    }
}
