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
			var hashset = new HashSet<string>(args);
			var tSuccess=0;
			var tFailed =0;
            var tIgnored = 0;
            var tTypes =
                Assembly.GetAssembly(typeof (Program)).GetTypes()
                    .Where(it => it.GetCustomAttributes(typeof (TestFixtureAttribute), false).Any());
			using(ImpromptuInterface.Build.BuildProxy.WriteOutDll("Emitted")){
			Console.WriteLine("Press enter to start.");
			Console.Read();
            foreach (var tType in tTypes.OrderBy(it => it.Name))
            {
                Console.WriteLine(tType.Name);
                var tMethods =
                    tType.GetMethods().Where(it => it.GetCustomAttributes(typeof (TestAttribute), false).Any());
                foreach (var tMethod in tMethods)
                {
                    var tObj = Activator.CreateInstance(tType);
                  	if(hashset.Any() && !hashset.Contains(String.Format("{0}.{1}",tType.Name,tMethod.Name))){
						continue;
					}
					
                    Console.Write("    ");
                    Console.WriteLine(tMethod.Name);
                    try
                    {
                        tMethod.Invoke(tObj,null);
                        Console.Write("       ");
                        Console.WriteLine("Success");
						tSuccess++;
                    }
                    catch (TargetInvocationException ex)
                    {
                       
                        if (ex.InnerException is AssertionException)
                        {
                            Console.Write("*      ");
                            Console.Write("Failed: ");
                            Console.WriteLine(ex.InnerException.Message);
                            Console.WriteLine();
							tFailed++;

                        }
                        else if (ex.InnerException is IgnoreException)
                        {
                            Console.Write("-      ");
                            Console.Write("Ignored: ");
                            Console.WriteLine(ex.InnerException.Message);
                            Console.WriteLine();
                            tIgnored++;
                        }
                        else
                        {
							tFailed++;
                            Console.Write("*      ");
							Console.Write("Exception: ");

                            Console.WriteLine(ex.InnerException.Message);
							Console.WriteLine(ex.InnerException.StackTrace);
							Console.WriteLine();
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
			Console.WriteLine("Done. Successes:{0} Failures:{1} Ignored:{2}",tSuccess,tFailed,tIgnored);
			Console.Read();
			}
        }

       
    }
}
