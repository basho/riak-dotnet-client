#!/bin/sh
PLATFORM=`uname -s`
if [ "Darwin" = "$PLATFORM" ]; then
    # OS/X install steps here
    if [ ! -x /usr/bin/mono ] || [ -z "$(mono -V | grep 3.8.0)" ]; then
        curl -O http://download.mono-project.com/archive/3.8.0/macos-10-x86/MonoFramework-MDK-3.8.0.macos10.xamarin.x86.pkg
        sudo /usr/sbin/installer -pkg MonoFramework-MDK-3.8.0.macos10.xamarin.x86.pkg -target /
    fi
else # Assume ubuntu/debian
	mozroots --import --sync
    sudo apt-get install -y python-software-properties
    sudo apt-add-repository -y ppa:directhex/monoxide
    sudo apt-get update
    sudo apt-get install -y mono-devel=3.2.1+dfsg-1~pre2 mono-gmcs=3.2.1+dfsg-1~pre2 nunit-console
fi