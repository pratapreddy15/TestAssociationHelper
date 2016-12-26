using Microsoft.SimplyAssociate.AppData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    internal class AssemblyHelper
    {
        AppDomain simplyAssociateDomain = null;
        AssemblyLoader loader = null;
        public AssemblyHelper(string domainName)
        {
            SetChildDomain(AppDomain.CurrentDomain, domainName);
            Type loaderType = typeof(AssemblyLoader);
            if (loaderType.Assembly != null)
            {
                loader = (AssemblyLoader)simplyAssociateDomain
                    .CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName)
                    .Unwrap();
            }
        }

        internal void SetChildDomain(AppDomain parentDomain, string domainName)
        {
            string assemblyPath = Path.Combine(parentDomain.SetupInformation.ApplicationBase, "PrivateAssemblies");
            Evidence evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setUp = parentDomain.SetupInformation;
            simplyAssociateDomain = AppDomain.CreateDomain(domainName, evidence, setUp);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return Assembly.Load(args.Name);
            }
            catch { }
            string[] nameParts = args.Name.Split(',');
            string path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            path = Path.Combine(path, nameParts[0]);
            return Assembly.LoadFile(path);
        }

        internal void UnloadDomain()
        {
            if (simplyAssociateDomain != null)
            {
                AppDomain.Unload(simplyAssociateDomain);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                simplyAssociateDomain = null;
            }
        }

        internal NewTestAssociationInfo BuildTestImplementationData(string pathContainingAssemblies, string fullPathOfAssembly,
            string nameOfTestClass, string nameOfTestMethod)
        {
            NewTestAssociationInfo testAssociationInfo = null;

            if (loader == null)
                return null;
            loader.LoadAssembly(fullPathOfAssembly);
            FileInfo assemblyFileInfo = new FileInfo(fullPathOfAssembly);
            string justAssemblyName = assemblyFileInfo.Name.Replace(assemblyFileInfo.Extension, string.Empty);
            testAssociationInfo = loader.GetNewTestAssociationData(
                pathContainingAssemblies,
                justAssemblyName,
                nameOfTestClass,
                nameOfTestMethod);

            return testAssociationInfo;
        }
    }
}
