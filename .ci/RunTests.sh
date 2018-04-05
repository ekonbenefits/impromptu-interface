#!/bin/sh -x

runTest(){
   mono --runtime=v4.0 packages/nunit.consolerunner/3.7.0/tools/nunit3-console.exe --noresult -labels=All "$@"
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

dotnet test Tests/UnitTestImpromptuInterface/UnitTestImpromptuInterface.csproj -f netcoreapp2.0 --no-build --no-restore --filter="TestCategory!=Performance" --configuration=Debug  
exit $?

runTest Tests/UnitTestImpromptuInterface/bin/Debug/net462/UnitTestImpromptuInterface.exe --where="cat != Performance"

runTest Tests/UnitTestImpromptuInterface.Clay/bin/Debug/UnitTestImpromptuInterface.Clay.dll --where="cat != Performance"

