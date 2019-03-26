# This repo has been archived, source code and issues moved to [AppMetrics](https://github.com/AppMetrics/AppMetrics)

# App Metrics Graphite <img src="http://app-metrics.io/logo.png" alt="App Metrics" width="50px"/> 
[![Official Site](https://img.shields.io/badge/site-appmetrics-blue.svg?style=flat-square)](http://app-metrics.io/reporting/influxdb.html) [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](https://opensource.org/licenses/Apache-2.0)

## What is it?

This repo contains Graphite extension packages to [App Metrics](https://github.com/alhardy/AppMetrics).

## Latest Builds, Packages & Repo Stats

|Branch|AppVeyor|Travis|Coverage|
|------|:--------:|:--------:|:--------:|
|dev|[![AppVeyor](https://img.shields.io/appveyor/ci/alhardy/graphite/dev.svg?style=flat-square&label=appveyor%20build)](https://ci.appveyor.com/project/alhardy/graphite/branch/dev)|[![Travis](https://img.shields.io/travis/alhardy/Graphite/dev.svg?style=flat-square&label=travis%20build)](https://travis-ci.org/alhardy/Graphite)|[![Coveralls](https://img.shields.io/coveralls/alhardy/Graphite/dev.svg?style=flat-square)](https://coveralls.io/github/alhardy/Graphite?branch=dev)
|master|[![AppVeyor](https://img.shields.io/appveyor/ci/alhardy/graphite/master.svg?style=flat-square&label=appveyor%20build)](https://ci.appveyor.com/project/alhardy/graphite/branch/master)| [![Travis](https://img.shields.io/travis/alhardy/Graphite/master.svg?style=flat-square&label=travis%20build)](https://travis-ci.org/alhardy/Graphite)| [![Coveralls](https://img.shields.io/coveralls/alhardy/Graphite/master.svg?style=flat-square)](https://coveralls.io/github/alhardy/Graphite?branch=master)|

|Package|Dev Release|PreRelease|Latest Release|
|------|:--------:|:--------:|:--------:|
|App.Metrics.Reporting.Graphite|[![MyGet Status](https://img.shields.io/myget/appmetrics/v/App.Metrics.Reporting.Graphite.svg?style=flat-square)](https://www.myget.org/feed/appmetrics/package/nuget/App.Metrics.Reporting.Graphite)|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Reporting.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Reporting.Graphite/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Reporting.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Reporting.Graphite/)
|App.Metrics.AspNetCore.Formatters.Graphite|[![MyGet Status](https://img.shields.io/myget/appmetrics/v/App.Metrics.AspNetCore.Formatters.Graphite.svg?style=flat-square)](https://www.myget.org/feed/appmetrics/package/nuget/App.Metrics.AspNetCore.Formatters.InfluxDB)|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.AspNetCore.Formatters.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.AspNetCore.Formatters.InfluxDB/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.AspNetCore.Formatters.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.AspNetCore.Formatters.Graphite/)
|App.Metrics.Formatters.Graphite|[![MyGet Status](https://img.shields.io/myget/appmetrics/v/App.Metrics.Formatters.Graphite.svg?style=flat-square)](https://www.myget.org/feed/appmetrics/package/nuget/App.Metrics.Formatters.Graphite)|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Formatters.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Formatters.Graphite/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Formatters.Graphite.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Formatters.Graphite/)

#### Grafana/Graphite Web Monitoring

![Grafana/Graphite Generic Web Dashboard Demo](https://github.com/alhardy/AppMetrics.DocFx/blob/master/images/generic_grafana_dashboard_demo.gif)

> Grab the dashboard [here](https://grafana.com/dashboards/2192)

#### Grafana/Graphite OAuth2 Client Monitoring on a Web API

![Grafana/Graphite Generic OAuth2 Web Dashboard Demo](https://github.com/alhardy/AppMetrics.DocFx/blob/master/images/generic_grafana_oauth2_dashboard_demo.gif)

> Grab the dashboard [here](https://grafana.com/dashboards/2198)

### Grafana/Graphite Web Application Setup

- Setup a [Graphite](https://hub.docker.com/r/ennexa/graphite/) container.
- Download and install [Grafana](https://grafana.com/grafana/download), then create a new [Graphite Datasource](http://docs.grafana.org/features/datasources/graphite/) and [import](http://docs.grafana.org/reference/export_import/#importing-a-dashboard) App.Metrics [web dashboard](https://grafana.com/dashboards/2192)
- Drop in the `App.Metrics.Extensions.Mvc` and `App.Metrics.Extensions.Reporting.Graphite` nuget packages into your web application. 
- Add [App.Metrics configuration](https://www.app-metrics.io/web-monitoring/aspnet-core/quick-start/) to the `Startup.cs` of your web application, including the [Graphite reporter configuration](https://www.app-metrics.io/reporting/reporters/graphite/). *You might want to check out the [Sandbox](https://github.com/AppMetrics/Graphite/tree/dev/sandbox/MetricsGraphiteSandbox) or [Sample](https://github.com/AppMetrics/AppMetrics/tree/dev/sandbox/MetricsSandbox) projects if you get stuck*
- Run your app and Grafana at visit `http://localhost:3000`

## How to build

[AppVeyor](https://ci.appveyor.com/project/alhardy/graphite/branch/master) and [Travis CI](https://travis-ci.org/alhardy/Graphite) builds are triggered on commits and PRs to `dev` and `master` branches.

See the following for build arguments and running locally.

|Configuration|Description|Default|Environment|Required|
|------|--------|:--------:|:--------:|:--------:|
|BuildConfiguration|The configuration to run the build, **Debug** or **Release** |*Release*|All|Optional|
|PreReleaseSuffix|The pre-release suffix for versioning nuget package artifacts e.g. `beta`|*ci*|All|Optional|
|CoverWith|**DotCover** or **OpenCover** to calculate and report code coverage, **None** to skip. When not **None**, a coverage file and html report will be generated at `./artifacts/coverage`|*OpenCover*|Windows Only|Optional|
|SkipCodeInspect|**false** to run ReSharper code inspect and report results, **true** to skip. When **true**, the code inspection html report and xml output will be generated at `./artifacts/resharper-reports`|*false*|Windows Only|Optional|
|BuildNumber|The build number to use for pre-release versions|*0*|All|Optional|


### Windows

Run `build.ps1` from the repositories root directory.

```
	.\build.ps1'
```

**With Arguments**

```
	.\build.ps1 --ScriptArgs '-BuildConfiguration=Release -PreReleaseSuffix=beta -CoverWith=OpenCover -SkipCodeInspect=false -BuildNumber=1'
```

### Linux & OSX

Run `build.sh` from the repositories root directory. Code Coverage reports are now supported on Linux and OSX, it will be skipped running in these environments.

```
	.\build.sh'
```

**With Arguments**

```
	.\build.sh --ScriptArgs '-BuildConfiguration=Release -PreReleaseSuffix=beta -BuildNumber=1'
```

## Contributing

See the [contribution guidlines](https://github.com/alhardy/AppMetrics/blob/master/CONTRIBUTING.md) in the [main repo](https://github.com/alhardy/AppMetrics) for details.

## Acknowledgements

* [DocFX](https://dotnet.github.io/docfx/)
* [Fluent Assertions](http://www.fluentassertions.com/)
* [XUnit](https://xunit.github.io/)
* [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
* [Cake](https://github.com/cake-build/cake)
* [OpenCover](https://github.com/OpenCover/opencover)

***Thanks for providing free open source licensing***

* [Jetbrains](https://www.jetbrains.com/dotnet/) 
* [AppVeyor](https://www.appveyor.com/)
* [Travis CI](https://travis-ci.org/)
* [Coveralls](https://coveralls.io/)

## License

This library is release under Apache 2.0 License ( see LICENSE ) Copyright (c) 2016 Allan Hardy
