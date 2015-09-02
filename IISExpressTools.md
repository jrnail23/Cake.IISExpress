## IIS Express Tool Docs ##

### iisexpress.exe ###

#### Usage ####
```
	iisexpress [/config:config-file] [/site:site-name] [/siteid:site-id] [/systray:true|false] [/trace:trace-level] [/userhome:user-home-directory]
	
	iisexpress /path:app-path [/port:port-number] [/clr:clr-version] [/systray:true|false] [/trace:trace-level]
```
#### Supported options ####
```
	/config:config-file
	The full path to the applicationhost.config file. The default value is the IISExpress\config\applicationhost.config file that is located in the user's Documents folder.

	/site:site-name
	The name of the site to launch, as described in the applicationhost.config file.

	/siteid:site-id
	The ID of the site to launch, as described in the applicationhost.config file.

	/path:app-path
	The full physical path of the application to run. You cannot combine this option with the /config and related options.

	/port:port-number
	The port to which the application will bind. The default value is 8080. You must also specify the /path option.

	/clr:clr-version
	The .NET Framework version (e.g. v2.0) to use to run the application. The default value is v4.0. You must also specify the /path option.

	/systray:true|false
	Enables or disables the system tray application. The default value is true.

	/userhome:user-home-directory
	IIS Express user custom home directory (default is %userprofile%\documents\iisexpress).

	/trace:trace-level
	Valid values are 'none', 'n', 'info', 'i', 'warning', 'w', 'error', and 'e'. The default value is none.
```

#### Examples ####
```
	iisexpress /site:WebSite1
	This command runs WebSite1 site from the user profile configuration file.

	iisexpress /config:c:\myconfig\applicationhost.config
	This command runs the first site in the specified configuration file.

	iisexpress /path:c:\myapp\ /port:80
	This command runs the site from the 'c:\myapp' folder over port '80'.
```

### IisExpressAdminCmd.exe ###

#### Usage ####
`iisexpressadmincmd.exe <command> <parameters>`

#### Supported commands ####
```
      setupFriendlyHostnameUrl -url:<url>
      deleteFriendlyHostnameUrl -url:<url>
      setupUrl -url:<url>
      deleteUrl -url:<url>
      setupSslUrl -url:<url> -CertHash:<value>
      setupSslUrl -url:<url> -UseSelfSigned
      deleteSslUrl -url:<url>
```

#### Examples ####
1) Configure "http.sys" and "hosts" file for friendly hostname "contoso":
```
    iisexpressadmincmd setupFriendlyHostnameUrl -url:http://contoso:80/
```
2) Remove "http.sys" configuration and "hosts" file entry for the friendly hostname "contoso":
```
    iisexpressadmincmd deleteFriendlyHostnameUrl -url:http://contoso:80/
```

### appcmd.exe ###