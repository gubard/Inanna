using System.Data.Common;
using System.Runtime.CompilerServices;
using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Services;
using Nestor.Db.Sqlite.Services;

namespace Inanna.Services;

public sealed class UiDatabaseFactory : IAdoDatabaseFactory
{
    public UiDatabaseFactory(
        AppState appState,
        IStorageService storageService,
        IMigrator migrator,
        string name
    )
    {
        _appState = appState;
        _storageService = storageService;
        _migrator = migrator;
        _name = name;
        _cache = new();
    }

    public ConfiguredValueTaskAwaitable<IDatabase<DbCommand>> CreateAsync(CancellationToken ct)
    {
        return CreateCore(ct).ConfigureAwait(false);
    }

    private readonly AppState _appState;
    private readonly IStorageService _storageService;
    private readonly Dictionary<string, IDatabase<DbCommand>> _cache;
    private readonly IMigrator _migrator;
    private readonly string _name;

    private async ValueTask<IDatabase<DbCommand>> CreateCore(CancellationToken ct)
    {
        var dbFile = CreateDbFile();
        await InitDbContextAsync(dbFile, ct).ConfigureAwait(false);

        return _cache[dbFile.FullName];
    }

    private FileInfo CreateDbFile()
    {
        if (_appState.User is null)
        {
            return new($"{_storageService.GetAppDictionary()}/{_name}.sqlitedb");
        }

        return new($"{_storageService.GetAppDictionary()}/{_appState.User.Id}.{_name}.sqlitedb");
    }

    private async ValueTask InitDbContextAsync(FileInfo file, CancellationToken ct)
    {
        if (_cache.ContainsKey(file.FullName))
        {
            return;
        }

        var database = new AdoDatabase(new FileSqliteDbConnectionFactory(file));
        await _migrator.MigrateAsync(database, ct);
        _cache.Add(file.FullName, database);
    }
}
