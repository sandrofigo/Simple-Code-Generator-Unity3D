# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.3] - 2025-08-15

### Added

- Re-added annotation attribute on CodeGenerationMethod attribute class

### Fixed

- Fixed assembly load error when using JetBrains Rider 2025.2

## [2.0.2] - 2025-08-14

### Fixed

- Fixed changelog format on release

## [2.0.1] - 2025-08-14

### Changed

- Log error instead of warning when a template file is not found

### Removed

- Removed annotation attribute on CodeGenerationMethod attribute class

## [2.0.0] - 2024-02-06

### Changed

- Changed automatic code generation to not run when only generated files have changed (containing `.g.` or `.generated.` in the file path)
- Moved core classes to separate assembly to be able to reference them in code outside of the editor

## [1.0.0] - 2023-08-27

### Added

- Initial release
