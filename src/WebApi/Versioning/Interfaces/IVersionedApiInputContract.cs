using MediatR;

namespace MyWarehouse.WebApi.Versioning.Interfaces
{
    /// <summary>
    /// Candidate interface for establishing discrete input models for each API version,
    /// with a method that converts to a stable application model.
    /// It's not used here, because currently it wouldn't provide any value to have another abstraction layer.
    /// </summary>
    internal interface IVersionedApiInputContract<TApplicationModel>
        where TApplicationModel : IBaseRequest
    {
        /// <summary>
        /// Converts a versioned API input contract to the current application model.
        /// </summary>
        TApplicationModel ToApplicationModel();
    }
}
