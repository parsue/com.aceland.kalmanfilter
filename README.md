## AceLand Kalman Filter

![Full Open Source](https://img.shields.io/badge/Full%20Open%20Source-2A8B22)
![ECS Ready](https://img.shields.io/badge/ECS%20Ready-4A5BC2)
![CoreCLR Ready](https://img.shields.io/badge/CoreCLR%20Ready-4A5BC2)   
![Sponsor](https://img.shields.io/badge/Sponsor-%E2%9D%A4-db61a2?logo=githubsponsors&amp;logoColor=white)
![Discord](https://img.shields.io/badge/Discord-Join-5865F2?logo=discord&amp;logoColor=white)
[![Docs](https://img.shields.io/badge/Docs-GitBook-3884FF?logo=gitbook&logoColor=white)](https://docs.parsue.io/aceland-unity-packages/core-packages/kalman-filter)

A lightweight, Burst-compatible Kalman filter for Unity. Every filter is a blittable `struct`, so it
runs inside Burst-compiled Jobs and ECS systems with zero managed allocation and no virtual dispatch.

## Quick Start
```csharp
using Aceland.KalmanFilter;

// Create a filter for a float signal.
var filter = Kalman.CreateFloatFilter();

// Feed measurements one by one.
float estimate = filter.Update(noisyValue);
```

## Burst / Jobs
Filters are value types (`KalmanFilter<T, TAdapter>`), so you can store them in a `NativeArray`
and schedule them with the built-in `KalmanBatchJob<T, TAdapter>`.

## Documents
We use GitBook as a public documents of our packages.

> Visit our [GitBook](https://docs.parsue.io/aceland-unity-packages/packages/kalman-filter)

Please visit our GitBook for details.
