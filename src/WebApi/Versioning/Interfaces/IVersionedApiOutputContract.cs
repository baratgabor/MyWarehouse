namespace MyWarehouse.WebApi.Versioning.Interfaces;

/// <summary>
/// Candidate interface for establishing discrete output models for each API version,
/// with a method that converts a stable application model to a versioned API output model.
/// It's not used here, because currently it wouldn't provide any value to have another abstraction layer.
/// </summary>
internal interface IVersionedApiOutputContract<TApplicationModel, TThis>
{
    /// <summary>
    /// Converts the current application model to versioned API output contract.
    /// </summary>
    TThis FromApplicationModel(TApplicationModel applicationModel);
}
