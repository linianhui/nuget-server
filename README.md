# Nuget Server

## `Web.config`中的重点配置项（默认）
```xml
<!--默认密码-->
<add key="apiKey" value="123456" />

<!--允许Symbols包-->
<add key="ignoreSymbolsPackages" value="false" />

<!--禁止覆盖已存在的包（id+version）-->
<add key="allowOverrideExistingPackageOnPush" value="false" />
```

## 部署

```powershell
./build.ps1
```

部署在iis中的站点：http://nuget-server.test/