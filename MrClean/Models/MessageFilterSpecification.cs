using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MrClean.Models;

public class MessageFilterSpecification
{
    /// <summary>
    ///     Character separating entities within the source string.
    ///     eg. <code>0;1;2</code>
    /// </summary>
    private const string Separator = ";";

    /// <summary>
    ///     Character implying that the entity should be negated (denied).
    ///     eg. <code>~1</code> will deny application of the filter for entity with ID of 1
    /// </summary>
    private const string Negation = "~";

    /// <summary>
    ///     Please do not use this property directly in your application code, use <see cref="AllowsEntity"/> instead
    /// </summary>
    public List<ulong> AllowedEntities { get; }

    /// <summary>
    ///     Please do not use this property directly in your application code, use <see cref="AllowsEntity"/> instead
    /// </summary>
    public List<ulong> DeniedEntities { get; }

    /// <summary>
    ///     Produced specification string that can be then stored in the database field
    ///     eg. <code>0;2;3;~4</code>
    /// </summary>
    public string? SpecificationString
    {
        get
        {
            if (AllowedEntities.Count == 0 && DeniedEntities.Count == 0)
            {
                return null;
            }

            var entities = new List<string>();
            
            entities.AddRange(AllowedEntities.Select(e => e.ToString()));
            entities.AddRange(DeniedEntities.Select(e => Negation + e));

            return string.Join(Separator, entities);
        }
    }

    public MessageFilterSpecification(string? sourceString)
    {
        AllowedEntities = new List<ulong>();
        DeniedEntities = new List<ulong>();

        // Empty or null source string means that this filter specification is applied to everything
        if (string.IsNullOrEmpty(sourceString))
        {
            return;
        }

        var entities = sourceString.Split(Separator);
        var allowed = entities.Where(e => !e.StartsWith(Negation)).Select(ulong.Parse).ToList();
        var denied = entities.Where(e => e.StartsWith(Negation))
            .Select(e => e.Replace(Negation, ""))
            .Select(ulong.Parse)
            .ToList();

        AllowedEntities = allowed;
        DeniedEntities = denied;
    }

    public bool AllowsEntity(ulong entity)
    {
        // Explicitly denied entities always return false
        if (DeniedEntities.Contains(entity))
        {
            return false;
        }

        // Otherwise, if there are no explicitly allowed entities, return true
        // If, on the other hand, there are explicitly allowed entities, only allow those
        return AllowedEntities.Count == 0 || AllowedEntities.Contains(entity);
    }

    public MessageFilterSpecification AddAllowedEntity(ulong entity)
    {
        AllowedEntities.Add(entity);
        DeniedEntities.Remove(entity);

        return this;
    }

    public MessageFilterSpecification AddDeniedEntity(ulong entity)
    {
        DeniedEntities.Add(entity);
        AllowedEntities.Remove(entity);

        return this;
    }
}