using System.Data;
using Dapper;
using PowerPointService.Models;
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

    public async Task<IEnumerable<VideosWithPresentation>> GetVideosWithPresentationAsync(Guid id, CancellationToken cancellationToken)
    {
        const string query =
            """                 
            SELECT 
                v.PresentationId,
                v.SlideId,
                v.Id,
                v.Name,
                v.FullFileName,
                v.Duration,
                p.Name AS PresentationName,
                p.FullFileName AS PresentationFileName,
                p.State AS PresentationState
            FROM Videos v
            INNER JOIN Presentations p ON v.PresentationId = p.Id
            WHERE v.PresentationId = @PresentationId
            """;

        return await this.dbConnection.QueryAsync<VideosWithPresentation>(query, new { PresentationId = id });
    }

    public async Task<VideoContentDto> GetVideoAsync(Guid id, CancellationToken cancellationToken)
    {
        const string query =
            """                 
            SELECT 
                Name, 
                FullFileName
            FROM Videos 
            WHERE Id = @VideoId
            """;

        return await this.dbConnection.QuerySingleOrDefaultAsync<VideoContentDto>(query, new { VideoId = id });
    }

    #endregion

    #region Private Methods

    #endregion
}