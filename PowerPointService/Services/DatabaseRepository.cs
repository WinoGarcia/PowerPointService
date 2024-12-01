using System.Data;
using Dapper;
using PowerPointService.Types;

namespace PowerPointService.Services;

public class DatabaseRepository : IDatabaseRepository
{
    #region Private Fields

    private readonly IDbConnection dbConnection;

    #endregion

    #region Constructors

    public DatabaseRepository(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
        this.dbConnection.Open();
    }

    #endregion

    #region Public Methods

    public async Task<int> InsertPresentationAsync(PresentationModel presentationModel, CancellationToken cancellationToken)
    {
        const string insertQuery =
            """
            INSERT INTO Presentations (Id, Name, FullFileName, State)
            VALUES (@Id, @Name, @FullFileName, @State);
            """;

        return await this.dbConnection.ExecuteAsync(insertQuery, presentationModel);
    }

    public async Task<int> UpdatePresentationStateAsync(Guid presentationId, PresentationStates state, CancellationToken cancellationToken)
    {
        const string updateQuery =
            """
            UPDATE Presentations
            SET State = @State
            WHERE Id = @Id
            """;
        return await this.dbConnection.ExecuteAsync(updateQuery, new { Id = presentationId, State = state });
    }

    public async Task<int> InsertVideosAsync(IEnumerable<VideoModel> videoModels, CancellationToken cancellationToken)
    {
        const string insertQuery =
            """
            INSERT INTO Videos 
            (PresentationId, SlideId, Id, Name, FullFileName, Duration)
            VALUES 
            (@PresentationId, @SlideId, @Id, @Name, @FullFileName, @Duration)
            """;

        return await this.dbConnection.ExecuteAsync(insertQuery, videoModels);
    }

    #endregion

    #region Private Methods

    #endregion
}