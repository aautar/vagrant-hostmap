**Consider this software experimental. It may not work with certain configuations and you should backup your hosts file before allowing the application to update it.**

## What it does

This application will get the IP address needed to connect to a Linux [Vagrant](https://www.vagrantup.com/) box and update the system's [hosts file](https://en.wikipedia.org/wiki/Hosts_(file)) with the correct IP ↔ hostname mapping. 


## Usage

Once installed, run the following within a directory that has a vagrant environment setup and **running**.

`vagrant-hostmap <hostname>`

## Installation

Run the installer, **vagrant-hostmap.msi**

The installation will create the necessary files/folders and update the PATH environment variable to add the folder where `vagrant-hostmap` is installed.

## Dependencies
The Linux system needs to be able to execute [ip](https://man7.org/linux/man-pages/man8/ip.8.html), specifically `ip -o -f inet addr`
