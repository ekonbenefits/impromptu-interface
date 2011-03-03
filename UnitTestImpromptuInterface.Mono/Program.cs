using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnitTestImpromptuInterface
{
    class Program
    {
        static void Main(string[] args)
        {

            var tTypes =
                Assembly.GetAssembly(typeof (Program)).GetTypes()
                    .Where(it => it.GetCustomAttributes(typeof (TestFixtureAttribute), false).Any());

            foreach (var tType in tTypes)
            {
                Console.WriteLine(tType.Name);
                var tMethods =
                    tType.GetMethods().Where(it => it.GetCustomAttributes(typeof (TestAttribute), false).Any());
                foreach (var tMethod in tMethods)
                {
                    var tObj = Activator.CreateInstance(tType);
                    Console.Write("    ");
                    Console.WriteLine(tMethod.Name);
                  
                    try
                    {
                        tMethod.Invoke(tObj,null);
                        Console.Write("       ");
                        Console.WriteLine("Success");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Console.Write("*      ");
                        if (ex.InnerException is AssertionException)
                        {

                            Console.Write("Failed: ");
                            Console.WriteLine(ex.InnerException.Message);
                            Console.WriteLine();
                        }
                        else
                        {
                            throw ex.InnerException;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:");
                        Console.Write(ex);
                        Console.WriteLine();
                    }
                }
               

            }
        }

       
    }
}
