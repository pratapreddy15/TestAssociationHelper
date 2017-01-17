using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SimplyAssociate.Utilities
{
    public class AssemblyLoader : MarshalByRefObject
    {
        public NewTestAssociationInfo GetNewTestAssociationData(string pathContainingAssemblies, string nameOfAssemblyWithoutExtension,
            string nameOfTestClass, string nameOfTestMethod)
        {
            NewTestAssociationInfo newTestAssociationInfo = null;
            DirectoryInfo directory = new DirectoryInfo(pathContainingAssemblies);
            ResolveEventHandler resolveEventHandler =
                (s, e) =>
                {
                    return OnReflectionOnlyResolve(
                        e, directory);
                };

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve
                += resolveEventHandler;

            Assembly reflectionOnlyAssembly =
                AppDomain.CurrentDomain.
                    ReflectionOnlyGetAssemblies().First();

            if (reflectionOnlyAssembly.GetName().Name.Equals(nameOfAssemblyWithoutExtension, StringComparison.OrdinalIgnoreCase))
            {
                Type requiredType = reflectionOnlyAssembly.GetType(nameOfTestClass);
                MethodInfo testMethod = requiredType.GetMethod(nameOfTestMethod);
                string testMethodFullName = string.Concat(testMethod.DeclaringType.FullName, ".", testMethod.Name);
                byte[] bytesTestMethodFullName = System.Text.Encoding.Unicode.GetBytes(testMethodFullName);
                SHA256 cryptoService = SHA256CryptoServiceProvider.Create();
                byte[] hashedTestMethod = cryptoService.ComputeHash(bytesTestMethodFullName);
                byte[] generatedGuid = new byte[16]; // Byte array for GUID must be exactly 16 bytes long
                Array.Copy(hashedTestMethod, generatedGuid, 16);
                Guid guidOfTestMethod = new Guid(generatedGuid);

                FileInfo assemblyFileInfo = new FileInfo(requiredType.Assembly.Location);

                newTestAssociationInfo = new NewTestAssociationInfo
                {
                    TestName = testMethodFullName,
                    Storage = requiredType.Assembly.GetName().Name + assemblyFileInfo.Extension,
                    TestId = guidOfTestMethod
                };
            }

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve
                -= resolveEventHandler;

            return newTestAssociationInfo;
        }

        private Assembly OnReflectionOnlyResolve(
            ResolveEventArgs args, DirectoryInfo directory)
        {

            Assembly loadedAssembly =
                AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                    .FirstOrDefault(
                      asm => string.Equals(asm.FullName, args.Name,
                          StringComparison.OrdinalIgnoreCase));

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            AssemblyName assemblyName =
                new AssemblyName(args.Name);
            string dependentAssemblyFilename =
                Path.Combine(directory.FullName,
                assemblyName.Name + ".dll");

            if (File.Exists(dependentAssemblyFilename))
            {
                return Assembly.ReflectionOnlyLoadFrom(
                    dependentAssemblyFilename);
            }
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public void LoadAssembly(string assemblyPath)
        {
            try
            {
                Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch (FileNotFoundException)
            {
                /* Continue loading assemblies even if an assembly
                 * can not be loaded in the new AppDomain. */
            }
        }
    }
}
