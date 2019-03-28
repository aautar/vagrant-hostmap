**Consider this software experimental. It may not work with certain configuations and you should backup your hosts file before allowing the application to update it.**

## What it does

This application will get the IP address needed to connect to a [Vagrant](https://www.vagrantup.com/) box and update the system's [hosts file](https://en.wikipedia.org/wiki/Hosts_(file)) with the correct IP ↔ hostname mapping. 


## Usage

Once installed, run the following within a directory that has a vagrant environment setup and **running**.

`vagrant-hostmap <hostname>`

## Installation

Compile the code, place the exe + related dependencies in directory, update the PATH environment var and add the directory.

(This will be simpler in the future, when a release done)
