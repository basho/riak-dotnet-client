#!/bin/sh

DIR=`dirname $0`
cd ${DIR}/..
make debug
rc=$?
echo "Make result: $rc"
exit $rc

