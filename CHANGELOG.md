# Changelog

## 0.2.0 (2026-03-28)

- Add column projection via `Columns` property on `CsvOptions`
- Add automatic delimiter detection via `CsvReader.DetectDelimiter`
- Add comment line support via `CommentChar` property on `CsvOptions`
- Add row filtering via `SkipRow` predicate on `CsvOptions`
- Add test project with xUnit tests
- Add GitHub issue templates, dependabot config, and PR template
- Update README with all 8 required badges and Support section

## 0.1.3 (2026-03-26)

- Add Sponsor badge and fix License link format in README

## 0.1.2 (2026-03-23)

- Fix NuGet badge URL format

## 0.1.1 (2026-03-23)

- Add ReadWithHeaders method to API documentation

## 0.1.0 (2026-03-22)

- Initial release
- RFC 4180 compliant CSV parsing with quoted fields and embedded newlines
- Streaming reader and writer for memory-efficient processing
- Reflection-based header-to-property mapping with type conversion
- Configurable delimiter, quote character, and culture
