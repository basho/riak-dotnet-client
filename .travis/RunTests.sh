#!/bin/sh

runTest(){
   mono ./packages/NUnit.Runners.2.6.3/tools/nunit-console.exe -nologo -nodots -labels $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

DIR=`dirname $0`
cd ${DIR}/..
runTest CorrugatedIron.Tests/bin/Debug/CorrugatedIron.Tests.dll

exit $?
