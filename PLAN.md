# FinnhubSdk v2.0 Implementation Plan

## Summary

Four changes for the next major release:
1. Fix double version increment on develop branch merges
2. Upgrade from .NET 9.0 to .NET 10.0
3. Convert `TimeoutSeconds`/`RetryDelayMilliseconds` to `TimeSpan`
4. Create sample console apps and simplify README

---

## Change 1: Fix Double Version Increment

### Problem
Both `ci.yml` and `pre-release.yml` trigger on `develop` branch pushes, causing duplicate builds.

### Solution
Remove `develop` from CI workflow's push triggers (keep it in pull_request triggers).

### Files to Modify
| File | Change |
|------|--------|
| `.github/workflows/ci.yml` | Line 5: Remove `develop` from push branches |

### Implementation
```yaml
# Before (line 5)
branches: [ main, develop, 'feature/**', 'phase*' ]

# After
branches: [ main, 'feature/**', 'phase*' ]
```

---

## Change 2: Upgrade to .NET 10.0

### Files to Modify
| File | Change |
|------|--------|
| `src/FinnhubSdk/FinnhubSdk.csproj` | `<TargetFramework>net10.0</TargetFramework>` |
| `tests/FinnhubSdk.Tests.Unit/FinnhubSdk.Tests.Unit.csproj` | `<TargetFramework>net10.0</TargetFramework>` |
| `Directory.Packages.props` | Update Microsoft.Extensions.* to 10.0.0, System.Text.Json to 10.0.0 |
| `.github/workflows/ci.yml` | Line 22: `dotnet-version: '10.0.x'` |
| `.github/workflows/pre-release.yml` | Line 20: `dotnet-version: '10.0.x'` |
| `.github/workflows/release.yml` | `dotnet-version: '10.0.x'` |
| `README.md` | Update ".NET 9.0" references to ".NET 10.0" |

### Package Version Updates (Directory.Packages.props)
```xml
<PackageVersion Include="Microsoft.Extensions.Configuration.Binder" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Http" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Http.Polly" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Options" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Options.DataAnnotations" Version="10.0.0" />
<PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
<PackageVersion Include="System.Text.Json" Version="10.0.0" />
```

---

## Change 3: Convert to TimeSpan

### Current State (FinnhubOptions.cs)
```csharp
public int TimeoutSeconds { get; set; } = 30;        // Lines 41-42
public int RetryDelayMilliseconds { get; set; } = 1000;  // Lines 53-54
```

### Target State
```csharp
public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
```

### Files to Modify
| File | Change |
|------|--------|
| `src/FinnhubSdk/Configuration/FinnhubOptions.cs` | Replace int properties with TimeSpan |
| `src/FinnhubSdk/Configuration/FinnhubOptionsValidator.cs` | Update validation for TimeSpan ranges |
| `src/FinnhubSdk/Extensions/ServiceCollectionExtensions.cs` | Use TimeSpan directly (remove conversions) |
| `src/FinnhubSdk/Infrastructure/Policies/FinnhubPolicies.cs` | Change parameters from int to TimeSpan |
| Unit tests | Update any tests using these properties |

### Validation Rules
- `Timeout`: 1 second to 5 minutes
- `RetryDelay`: 100 milliseconds to 1 minute

---

## Change 4: Sample Console Apps

### New Files to Create
```
samples/
├── FinnhubSdk.Samples.Rest/
│   ├── FinnhubSdk.Samples.Rest.csproj
│   ├── Program.cs
│   └── appsettings.json
└── FinnhubSdk.Samples.WebSocket/
    ├── FinnhubSdk.Samples.WebSocket.csproj
    ├── Program.cs
    └── appsettings.json
```

### REST Sample Features
Demonstrate all REST endpoints:
- Stock quotes (`GetQuoteAsync`)
- Company profiles (`GetCompanyProfileAsync`)
- Historical candles (`GetCandlesAsync`)
- Symbol search (`SearchSymbolsAsync`)
- Company news (`GetCompanyNewsAsync`)
- Market news (`GetMarketNewsAsync`)
- News sentiment (`GetNewsSentimentAsync`)

### WebSocket Sample Features
Demonstrate real-time streaming:
- Connect/disconnect
- Subscribe to multiple symbols
- Handle trade callbacks
- Connection state changes
- Error handling
- Graceful shutdown with Ctrl+C

### Environment Variables
- `FINNHUB_API_KEY` - API key for both samples

### Files to Modify
| File | Change |
|------|--------|
| `FinnhubSdk.slnx` | Add `/samples/` folder with both projects |
| `Directory.Packages.props` | Add `Microsoft.Extensions.Hosting` Version="10.0.0" |
| `README.md` | Remove inline code examples, add samples section |

### README Simplification
Current: ~445 lines with extensive code examples
Target: ~150-200 lines with:
- Brief description and badges
- Installation
- Quick start (minimal example)
- Link to samples folder
- Configuration options table
- Error handling overview
- Requirements

---

## Implementation Order

1. **Change 1**: Fix CI workflow (independent, low risk)
2. **Change 3**: TimeSpan conversion (breaking change, do early)
3. **Change 2**: .NET 10 upgrade (depends on package availability)
4. **Change 4**: Sample apps (do after other changes are stable)

---

## Verification

### After Change 1
- Create feature branch, verify CI runs
- Create PR to develop, verify CI runs on PR
- Merge PR, verify ONLY pre-release.yml runs

### After Change 2
- `dotnet build` succeeds
- `dotnet test` all tests pass
- `dotnet pack` creates package

### After Change 3
- Build succeeds with no warnings
- All unit tests pass
- Configuration binding works with JSON TimeSpan strings (e.g., `"00:01:00"`)

### After Change 4
- `dotnet run --project samples/FinnhubSdk.Samples.Rest` executes successfully
- `dotnet run --project samples/FinnhubSdk.Samples.WebSocket` connects and receives trades
- README is concise and links work

---

## Critical Files Summary

- `src/FinnhubSdk/Configuration/FinnhubOptions.cs` - TimeSpan changes
- `src/FinnhubSdk/Extensions/ServiceCollectionExtensions.cs` - DI registration
- `src/FinnhubSdk/Infrastructure/Policies/FinnhubPolicies.cs` - Polly policies
- `.github/workflows/ci.yml` - Workflow fix + .NET version
- `.github/workflows/pre-release.yml` - .NET version
- `.github/workflows/release.yml` - .NET version
- `Directory.Packages.props` - Package versions
- `README.md` - Documentation updates