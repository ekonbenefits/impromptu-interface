#!/bin/sh -x

runTest(){
    mono --runtime=v4.0 packages/nunit.runners/2.6.1/tools/nunit-console.exe -noxml -nodots -labels $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

runTest Tests/UnitTestImpromptuInterface/bin/Debug/UnitTestImpromptuInterface.dll -exclude=Performance
runTest Tests/UnitTestImpromptuInterface.Clay/bin/Debug/UnitTestImpromptuInterface.Clay.dll -exclude=Performance

exit $?
