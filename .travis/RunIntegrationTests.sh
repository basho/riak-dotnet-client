#!/bin/sh

runTest(){
   nunit-console -nologo -nodots -labels $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

DIR=`dirname $0`
cd ${DIR}/..
runTest CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll

exit $?
