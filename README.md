## AceLand Kalman Filter
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

> Visit our [GitBook](https://aceland-workshop.gitbook.io/aceland-unity-packages/)

Please visit our GitBook for details.
