#!/bin/sh
DIR=`dirname $0`
export EnableNuGetPackageRestore=true

mkdir -p ${DIR}/../packages
cd ${DIR}/../packages
for i in CorrugatedIron CorrugatedIron.Tests CorrugatedIron.Tests.Live
  do mono --runtime=v4.0 ../.nuget/NuGet.exe install ../$i/packages.config
done
exit 0
