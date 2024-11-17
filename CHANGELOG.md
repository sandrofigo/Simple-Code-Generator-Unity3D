# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Log error instead of warning when a template file is not found

## [2.0.0] - 2024-02-06

### Changed

- Changed automatic code generation to not run when only generated files have changed (containing `.g.` or `.generated.` in the file path)
- Moved core classes to separate assembly to be able to reference them in code outside of the editor

## [1.0.0] - 2023-08-27

### Added

- Initial release
