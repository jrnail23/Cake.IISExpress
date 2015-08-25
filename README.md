# Cake.IISExpress #

## Tool Docs ##
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


