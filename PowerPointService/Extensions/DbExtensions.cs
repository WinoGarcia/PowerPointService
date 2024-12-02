using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using PowerPointService.Models;
using PowerPointService.Services;

namespace PowerPointService.Extensions;

public static class DbExtensions
{
    #region Consts

    /// <see cref="PresentationModel"/>
    private const string createPresentationsTable =
        """
        CREATE TABLE Presentations (
            Id TEXT PRIMARY KEY,
            Name TEXT NOT NULL,
            FullFileName TEXT NOT NULL,
            State INTEGER NOT NULL
        );
        """;

    /// <see cref="VideoModel"/>
    private const string createVideosTable =
        """
        CREATE TABLE Videos (
            Id TEXT PRIMARY KEY,
            PresentationId TEXT NOT NULL,
            SlideId INTEGER NOT NULL,
            Name TEXT NOT NULL,
            FullFileName TEXT NOT NULL,
            Duration TEXT NOT NULL,
            FOREIGN KEY (PresentationId) REFERENCES Presentations(Id)
        );
        """;

    #endregion

    #region Public Methods

    public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDbConnection>(_ =>
        {
            var connection = new SqliteConnection("Data Source=InMemory;Mode=Memory;Cache=Shared");
            connection.Open();
            InitializeDatabase(connection);
            return connection;
        });

        serviceCollection.AddScoped<IDatabaseRepository, DatabaseRepository>();

        return serviceCollection;
    }

    #endregion

    #region Private Methods

    private static void InitializeDatabase(IDbConnection dbConnection)
    {
        dbConnection.Execute(createPresentationsTable);
        dbConnection.Execute(createVideosTable);
    }

    #endregion
}