# ProxyMiner
[![Build status](https://ci.appveyor.com/api/projects/status/voi6r3ra0dgcxwe5/branch/master?svg=true)](https://ci.appveyor.com/project/iborzenkov/proxyminer/branch/master)

.NET Proxy Miner
ProxyMiner searches for proxies in the specified sources and checks their status and level of anonymity by connecting to the specified sites through them.

The sources can be text files or various websites.
It is possible to add your own source by implementing support for the IProxyProvider interface.

The algorithm for checking the proxy for anonymity can also be carried out independently. To do this, you need to implement the IChecker interface.
In the provided simple implementation, a connection is made to real-life sites on the Internet. 
After successful connection, the anonymity level of the proxy on the resource is determined http://www.whatismyip.cz/ 
If the real IP address could not be determined, the proxy is assigned the status "anonymous".