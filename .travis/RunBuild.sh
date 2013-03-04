#!/bin/sh -x

DIR=`dirname $0`
cd ${DIR}/..
make debug
echo "Make result: $?"
exit $?

