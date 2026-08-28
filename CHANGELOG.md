# Changelog

All notable changes to this project will be documented in this file.

---

## [3.0.0] - 2026-8-28
### Changed (Breaking)
- [Kalman Filter] reworked into a blittable `struct KalmanFilter<T, TAdapter>` - now fully Burst / Jobs / ECS compatible with zero managed allocation and no virtual dispatch.
- [Value Adapter] constraint changed from `struct` to `unmanaged`; all built-in adapters are now `public readonly struct` so they can be used as the `TAdapter` generic argument.
- [IKalmanFilter] batch `Update` now takes a `NativeArray<T>` instead of `List<T>`; single `Update` uses `float.NaN` sentinels instead of `float?` for optional noise values.
- [Factory] `Kalman.CreateXxxFilter()` now return the concrete `KalmanFilter<T, TAdapter>` value type. Custom types use `Kalman.Create<T, TAdapter>(adapter)`.
### Added
- [Jobs] `KalmanBatchJob<T, TAdapter>` - a ready-to-use `IJobParallelFor` running one filter per element, with `RegisterGenericJobType` pre-registered for all built-in value types.
- [Dependencies] com.unity.collections for `NativeArray` support.
### Removed
- [Core] `KalmanFilterBase<T>` abstract class (no longer needed for a value-type design).

## [2.2.1] - 2025-12-13
### Added
- all meta files

## [2.2.0] - 2025-12-13
### Removed
- all meta files

## [2.0.11] - 2025-12-06
### Modified
- [package.json] add doc links

## [2.0.10] - 2025-11-30
### Fixed
- [Version] fixing version in OpenUPM

## [1.0.3] - 2025-11-30
### Fixed
- [Burst Kalman Filter] Build() is not exposed

## [1.0.2] - 2025-11-30
### Added
- [Burst Kalman Filter] Jobs and ECS ready now.

## [1.0.1] - 2025-11-30
### Added
- [Value Adapter] customizable value processor, supporting any unmanaged types
- [Value Adapter] default value type - float, Vector2, Vector3, Vector4
### Modified
- [Builder] simplify builder with single constructor function Build()
### Removed
- [Builder] no builder chains due to rarely change default noise values q, r and p

## [1.0.0] - 2025-11-30
First public release. Move from Aceland Library.
For detail please visit and bookmark our [GitBook](https://aceland-workshop.gitbook.io/aceland-unity-packages/)
