using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Features.Finding.Usages;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Search;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.UI.Avalon.TreeListView;
using JetBrains.Util;

namespace LazyLoader
{
    [ContextAction(Description = "Use Lazy<T> Value",
      Group = "C#",
      Name = "Use Lazy<T> Value")]
    public sealed class UseLazyValueAction : ContextActionBase
    {
        private readonly ICSharpContextActionDataProvider _provider;
        private ICSharpTypeMemberDeclaration _parameterDeclaration;

        /// <summary>
        /// For languages other than C# any inheritor of <see cref="IContextActionDataProvider"/> can 
        /// be injected in this constructor.
        /// </summary>
        public UseLazyValueAction(ICSharpContextActionDataProvider provider)
        {
            _provider = provider;
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var declaration = _provider.GetSelectedElement<IFieldDeclaration>(false, true);
            if (declaration == null || !declaration.IsValid()) return false;

            var className = declaration.Type.GetLongPresentableName(CSharpLanguage.Instance);
            if (className != null && className.StartsWith("System.Lazy"))
            {
                _parameterDeclaration = declaration;
                return true;
            }

            return false;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var alltext = _provider.SourceFile.Document.GetText();
            var firstOcc = alltext.IndexOf(_parameterDeclaration.DeclaredName, StringComparison.Ordinal);
            if (firstOcc == -1) return null;

            var secondOcc = alltext.IndexOf(_parameterDeclaration.DeclaredName, firstOcc + _parameterDeclaration.DeclaredName.Length, StringComparison.Ordinal);
            if (secondOcc == -1) return null;

            var firstPart = alltext.Substring(0, secondOcc + _parameterDeclaration.DeclaredName.Length);
            var secondPart = alltext.Substring(secondOcc + _parameterDeclaration.DeclaredName.Length + 1);
            secondPart = secondPart.Replace(_parameterDeclaration.DeclaredName, _parameterDeclaration.DeclaredName + ".Value");
            var newText = firstPart + secondPart;
            _provider.SourceFile.Document.SetText(newText);

            return null;
        }

        public override string Text
        {
            get { return "Use Lazy<T> Value"; }
        }
    }
}
