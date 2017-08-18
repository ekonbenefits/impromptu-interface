#!/bin/sh -x

runTest(){
   mono --runtime=v4.0 packages/nunit.consolerunner/3.7.0/tools/nunit3-console.exe --noresult -labels=All "$@"
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

runTest Tests/UnitTestImpromptuInterface/bin/Debug/UnitTestImpromptuInterface.exe --where="cat != Performance"
runTest Tests/UnitTestImpromptuInterface.Clay/bin/Debug/UnitTestImpromptuInterface.Clay.dll --where="cat != Performance"
dotnet test Tests/Tests.csproj -f netcoreapp2.0 --no-build --no-restore --filter=TestCategory!=Performance --logger=trx;LogFileName=testresults.trx --configuration=Debug  
exit $?
