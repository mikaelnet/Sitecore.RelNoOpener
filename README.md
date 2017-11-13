# Sitecore.RelNoOpener

## Brief
Adds `rel="noopener"` to external links in Sitecore.

## How it works
When linking to external websites into a new window or tab, the `window.opener` references the source site, making this a security risk. More info and example of this problem can be found [here](https://www.jitbit.com/alexblog/256-targetblank---the-most-underestimated-vulnerability-ever/) and [here](https://mathiasbynens.github.io/rel-noopener/).

The module is a code example of how to solve this in Sitecore. It adds the `rel="noopener"` tag when rendering external link fields of the Sitecore types "Generic Link" and "Generic Link with Search". It also adds the `rel="noopener"` to external links withing rich text fields at save time.

To support older browsers, you may set `Stendahls.Sc.RelNoOpener.AddNoReferrer` to `true`. The module will then render `rel="noopener noreferrer"` on such links. If you're using Sitecore 8.2u5 or later, Sitecore has built-in support for adding these `rel` attributes. However, some people have found problem with Sitecores implementation. If you use this module, it will disable Sitecores implementation by setting `ProtectExternalLinksWithBlankTarget` to `false`. 

## Building
Build the solution and copy the .dll and .config files into your solution. Alternatively, you may just grab the pre-made [Sitecore package](https://github.com/mikaelnet/Sitecore.RelNoOpener/raw/master/Stendahls.RelNoOpener-1.0.zip) and install it.

## Compatibility
This code is pretty generic and will probably work on most versions of Sitecore, though it's not very well tested. You might need to change the version reference to Sitecore.Kernel and HtmlAgilityPack. When possible, use the same version of HtmlAgilityPack as your Sitecore version uses, or you'd have to add an assembly binding reference in your web.config.

